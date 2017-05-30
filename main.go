package main

import (
	"bytes"
	"context"
	"errors"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"path"
	"time"

	"github.com/unidoc/unidoc/pdf"
	"golang.org/x/net/context/ctxhttp"
)

var (
	errDecryptionFailed = errors.New("Failed to decrypt PDF page")
	errNoPages          = errors.New("No pages found in archive")
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
			return nil, err
		}

		pages = append(pages, page)
	}

	return pages, nil
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
			return nil, errDecryptionFailed
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

	session, err := Login(ctx, login, password)

	if err != nil {
		log.Fatal(err)
	}

	magazines, err := session.GetMagazines(ctx)

	if err != nil {
		log.Fatal(err)
	}

	for _, magazine := range magazines {
		os.Mkdir(magazine.Title, 0755)

		for _, issueID := range magazine.Issues {
			issue, err := session.GetIssue(ctx, magazine.ID, issueID)

			if err != nil {
				log.Fatal(err)
			}

			pages, err := downloadAllPages(context.Background(), *issue)

			if err != nil {
				log.Fatal(err)
			}

			pdf, err := unlockAndMerge(pages, []byte(issue.Password))

			if err != nil {
				log.Fatal(err)
			}

			err = func() (err error) {
				path := path.Join(magazine.Title, issue.Title+".pdf")
				file, err := os.Create(path)

				if err != nil {
					return err
				}

				defer func() {
					if cerr := file.Close(); cerr != nil && err == nil {
						err = cerr
					}
				}()

				return pdf.Write(file)
			}()

			if err != nil {
				log.Fatal(err)
			}
		}
	}
}
