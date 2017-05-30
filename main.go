package main

import (
	"bytes"
	"context"
	"flag"

	"io/ioutil"
	"net/http"
	"os"
	"path"
	"time"

	"github.com/pkg/errors"
	"github.com/unidoc/unidoc/pdf"
	"golang.org/x/net/context/ctxhttp"

	log "github.com/sirupsen/logrus"
)

var (
	downloadTimeout = 5 * time.Minute
)

func downloadPage(ctx context.Context, url string) (page, error) {
	resp, err := ctxhttp.Get(ctx, http.DefaultClient, url)

	if err != nil {
		return page{}, err
	}

	defer resp.Body.Close()
	b, err := ioutil.ReadAll(resp.Body)

	if err != nil {
		return page{}, err
	}

	r := bytes.NewReader(b)
	return page{r}, nil
}

func downloadAllPages(ctx context.Context, issue *Issue) ([]page, error) {
	ctx, cancel := context.WithTimeout(ctx, downloadTimeout)
	defer cancel()

	var pages []page

	for i := 0; i < issue.PageCount; i++ {
		url, err := issue.GetURL(i)

		if err != nil {
			return nil, err
		}

		page, err := downloadPage(ctx, url)

		if err != nil {
			return nil, errors.Wrapf(err, "failed to download page %d", i)
		}

		pages = append(pages, page)
	}

	return pages, nil
}

func downloadAllIssues(ctx context.Context, session *Session, magazines []Magazine) error {
	for _, magazine := range magazines {
		for _, issueID := range magazine.Issues {
			log.WithFields(log.Fields{"magazine": magazine.Title, "issue": issueID}).Info("downloading issue metadata")
			issue, err := session.GetIssue(ctx, magazine.ID, issueID)

			if err != nil {
				return err
			}

			path := path.Join(magazine.Title, issue.Title+".pdf")

			if _, err := os.Stat(path); err == nil {
				log.WithFields(log.Fields{"magazine": magazine.Title, "issue": issue.Title}).Info("issue already downloaded")
				continue
			}

			log.WithFields(log.Fields{"magazine": magazine.Title, "issue": issue.Title}).Info("downloading issue")
			pages, err := downloadAllPages(ctx, issue)

			if err != nil {
				return errors.Wrapf(err, "failed to download %s", path)
			}

			log.WithField("path", path).Info("saving issue")

			if _, err := os.Stat(magazine.Title); os.IsNotExist(err) {
				if err := os.Mkdir(magazine.Title, 0755); err != nil {
					return errors.Wrapf(err, "failed to create directory %s", magazine.Title)
				}
			}

			if err := save(session, pages, issue.Password, path); err != nil {
				return err
			}
		}
	}

	return nil
}

func save(session *Session, pages []page, password string, path string) (err error) {
	pdf, err := unlockAndMerge(pages, []byte(password))

	if err != nil {
		return errors.Wrapf(err, "failed to unlock and merge pages for %s", path)
	}

	file, err := os.Create(path)

	if err != nil {
		return errors.Wrapf(err, "failed to create %s", path)
	}

	defer func() {
		if cerr := file.Close(); cerr != nil && err == nil {
			err = cerr
		}
	}()

	if err = pdf.Write(file); err != nil {
		return errors.Wrapf(err, "failed to save %s", path)
	}

	return nil
}

func unlockAndMerge(pages []page, password []byte) (*pdf.PdfWriter, error) {
	w := pdf.NewPdfWriter()

	for _, page := range pages {
		r, err := pdf.NewPdfReader(page)

		if err != nil {
			return nil, err
		}

		ok, err := r.Decrypt(password)

		if err != nil {
			return nil, err
		}

		if !ok {
			return nil, errors.Errorf("failed to decrypt pages using password %s", string(password))
		}

		numPages, err := r.GetNumPages()

		if err != nil {
			return nil, err
		}

		for i := 0; i < numPages; i++ {
			page, err := r.GetPage(i + 1)

			if err != nil {
				return nil, err
			}

			if err = w.AddPage(page); err != nil {
				return nil, err
			}
		}
	}

	return &w, nil
}

func main() {
	var login, password string

	flag.StringVar(&login, "email", "", "Account email")
	flag.StringVar(&password, "password", "", "Account password")

	flag.Parse()

	if login == "" {
		login = os.Getenv("ZINIO_EMAIL")
	}

	if password == "" {
		password = os.Getenv("ZINIO_PASSWORD")
	}

	if login == "" || password == "" {
		flag.Usage()
		os.Exit(1)
	}

	ctx := context.Background()

	log.WithField("user", login).Info("logging in")
	session, err := Login(ctx, login, password)

	if err != nil {
		log.Fatal(err)
	}

	log.Info("downloading list of all magazines")
	magazines, err := session.GetMagazines(ctx)

	if err != nil {
		log.Fatal(err)
	}

	if err = downloadAllIssues(ctx, session, magazines); err != nil {
		log.Fatal(err)
	}
}
