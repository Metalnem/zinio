package main

import (
	"bytes"
	"context"

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

func downloadAllPages(ctx context.Context, issue Issue) ([]page, error) {
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

func downloadIssue(ctx context.Context, session *Session, issue Issue, path string) (err error) {
	pages, err := downloadAllPages(context.Background(), issue)

	if err != nil {
		return errors.Wrapf(err, "failed to download %s", path)
	}

	pdf, err := unlockAndMerge(pages, []byte(issue.Password))

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

func downloadAllIssues(ctx context.Context, session *Session, magazines []Magazine) error {
	for _, magazine := range magazines {
		if len(magazine.Issues) == 0 {
			continue
		}

		os.Mkdir(magazine.Title, 0755)

		for _, issueID := range magazine.Issues {
			log.WithFields(log.Fields{
				"magazine": magazine.Title,
				"issue":    issueID,
			}).Info("downloading issue metadata")

			issue, err := session.GetIssue(ctx, magazine.ID, issueID)

			if err != nil {
				return err
			}

			path := path.Join(magazine.Title, issue.Title+".pdf")

			log.WithFields(log.Fields{
				"magazine": magazine.Title,
				"issue":    issue.Title,
			}).Info("downloading issue")

			if err := downloadIssue(ctx, session, *issue, path); err != nil {
				return err
			}
		}
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
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Minute)
	defer cancel()

	login := os.Getenv("ZINIO_EMAIL")
	password := os.Getenv("ZINIO_PASSWORD")

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

	if err = downloadAllIssues(context.Background(), session, magazines); err != nil {
		log.Fatal(err)
	}
}
