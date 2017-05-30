package main

import (
	"bytes"
	"context"
	"crypto/aes"
	"crypto/cipher"
	"encoding/base64"
	"encoding/xml"
	"fmt"
	"io/ioutil"
	"net/http"
	"time"

	"github.com/cenkalti/backoff"
	"github.com/pkg/errors"
	"golang.org/x/net/context/ctxhttp"
)

const (
	baseURL        = "https://services.zinio.com/newsstandServices/"
	httpTimeout    = 5 * time.Second
	requestTimeout = time.Minute

	deviceID         = "A6B50079-BE65-44D6-A961-8A184AA81077"
	installationUUID = "84A8E36D-DF7F-4D7E-9824-F793AFB93207"
)

var (
	decryptionKey = []byte("8D}[" + deviceID[0:4] + "i)|z" + installationUUID[0:4])
)

// MagazineID is a unique identifier of a magazine.
type MagazineID string

// IssueID is a unique identifier of an issue.
type IssueID string

// Session contains session data for single authenticated user.
type Session struct {
	login     string
	password  string
	profileID string
}

// Magazine represents single magazine with all its issues.
type Magazine struct {
	ID     MagazineID
	Title  string
	Issues []IssueID
}

// Issue represents single magazine issue.
type Issue struct {
	ID        IssueID
	Title     string
	Password  string
	PageCount int
	baseURL   string
}

type parameters struct {
	login     string
	password  string
	profileID string
	pubID     MagazineID
	issueID   IssueID
}

type page struct {
	*bytes.Reader
}

type version struct {
	Key   string `xml:"key,attr"`
	Value string `xml:"value,attr"`
}

type libraryIssueDataRequest struct {
	PubID   MagazineID `xml:"pubId"`
	IssueID IssueID    `xml:"issueId"`
}

type zinioServiceRequest struct {
	XMLName       xml.Name `xml:"zinioServiceRequest"`
	RequestHeader struct {
		Authorization struct {
			Login    string `xml:"login"`
			Password string `xml:"password"`
		} `xml:"authorization"`
		Device struct {
			ProfileID           string `xml:"profileId,omitempty"`
			DeviceID            string `xml:"deviceId"`
			DeviceName          string `xml:"deviceName"`
			InstallationUUID    string `xml:"installationUUID"`
			PlatformDescription string `xml:"platformDescription"`
		} `xml:"device"`
		Application struct {
			ApplicationName    string    `xml:"applicationName"`
			ApplicationVersion string    `xml:"applicationVersion"`
			Versions           []version `xml:"versions>version"`
		} `xml:"application"`
	} `xml:"requestHeader"`
	LibraryIssueDataRequest *libraryIssueDataRequest `xml:"libraryIssueDataRequest,omitempty"`
}

type authenticateUserResponse struct {
	ProfileID string `xml:"profileId"`
}

type library struct {
	LibraryPublications []struct {
		Publication struct {
			PubID       string `xml:"pubId"`
			DisplayName string `xml:"displayName"`
		} `xml:"publication"`
		LibraryIssue struct {
			Issue struct {
				IssueID string `xml:"issueId"`
				Title   string `xml:"title"`
			} `xml:"issue"`
		} `xml:"libraryIssue"`
	} `xml:"libraryPublication"`
}

type issuePackingList struct {
	SingleIssue struct {
		PubID         string `xml:"pubId"`
		IssueTitle    string `xml:"issueTitle"`
		IssueID       string `xml:"issueId"`
		HostName      string `xml:"hostName"`
		IssueAssetDir string `xml:"issueAssetDir"`
		TrackingCode  struct {
			Init  string `xml:"init,attr"`
			Value string `xml:",chardata"`
		} `xml:"trackingCode"`
		NumberOfPages int `xml:"numberOfPages"`
	} `xml:"singleIssue"`
}

type zinioServiceResponse struct {
	XMLName        xml.Name `xml:"zinioServiceResponse"`
	ResponseStatus struct {
		Status      string `xml:"status"`
		ErrorDetail struct {
			Code    string `xml:"code"`
			Message string `xml:"message"`
		} `xml:"errorDetail"`
	} `xml:"responseStatus"`
	AuthenticateUserResponse authenticateUserResponse `xml:"authenticateUserResponse"`
	Library                  library                  `xml:"library"`
	IssuePackingList         issuePackingList         `xml:"issuePackingList"`
}

func makeRequest(p parameters) zinioServiceRequest {
	var req zinioServiceRequest
	h := &req.RequestHeader

	h.Authorization.Login = p.login
	h.Authorization.Password = p.password

	h.Device.ProfileID = p.profileID
	h.Device.DeviceID = deviceID
	h.Device.DeviceName = "Windows 8 Device"
	h.Device.InstallationUUID = installationUUID
	h.Device.PlatformDescription = "Windows 8"

	h.Application.ApplicationName = "ZinioWin8"
	h.Application.Versions = []version{{"security", "1.0"}}

	if p.pubID != "" && p.issueID != "" {
		req.LibraryIssueDataRequest = &libraryIssueDataRequest{
			PubID:   p.pubID,
			IssueID: p.issueID,
		}
	}

	return req
}

func retry(ctx context.Context, query string, p parameters) (*zinioServiceResponse, error) {
	ctx, cancel := context.WithTimeout(ctx, requestTimeout)
	defer cancel()

	var resp *zinioServiceResponse
	var err error

	op := func() error {
		resp, err = post(ctx, query, p)
		return err
	}

	b := backoff.WithContext(backoff.NewConstantBackOff(time.Second), ctx)

	if err := backoff.Retry(op, b); err != nil {
		return nil, err
	}

	return resp, nil
}

func post(ctx context.Context, query string, p parameters) (*zinioServiceResponse, error) {
	ctx, cancel := context.WithTimeout(ctx, httpTimeout)
	defer cancel()

	req := makeRequest(p)
	b, err := xml.Marshal(req)

	if err != nil {
		return nil, err
	}

	r := bytes.NewReader(b)
	resp, err := ctxhttp.Post(ctx, http.DefaultClient, baseURL+query, "text/xml", r)

	if err != nil {
		return nil, err
	}

	defer resp.Body.Close()
	b, err = ioutil.ReadAll(resp.Body)

	if err != nil {
		return nil, err
	}

	var res zinioServiceResponse

	if err = xml.Unmarshal(b, &res); err != nil {
		return nil, err
	}

	message := res.ResponseStatus.ErrorDetail.Message

	if message != "" {
		return nil, errors.New(message)
	}

	return &res, nil
}

func decryptPdfPassword(iv64, ciphertext64 string) (string, error) {
	iv, err := base64.StdEncoding.DecodeString(iv64)

	if err != nil {
		return "", err
	}

	ciphertext, err := base64.StdEncoding.DecodeString(ciphertext64)

	if err != nil {
		return "", err
	}

	block, err := aes.NewCipher(decryptionKey)

	if err != nil {
		return "", err
	}

	mode := cipher.NewCBCDecrypter(block, iv)
	mode.CryptBlocks(ciphertext, ciphertext)

	return string(ciphertext[:32]), nil
}

// Login connects to Zinio API server and authenticates user using given email and password.
func Login(ctx context.Context, email, password string) (*Session, error) {
	p := parameters{
		login:    email,
		password: password,
	}

	resp, err := retry(ctx, "authenticateUser", p)

	if err != nil {
		return nil, errors.Wrapf(err, "failed to authenticate user %s", email)
	}

	session := Session{
		login:     email,
		password:  password,
		profileID: resp.AuthenticateUserResponse.ProfileID,
	}

	return &session, nil
}

// GetMagazines downloads list of all available magazines.
func (session *Session) GetMagazines(ctx context.Context) ([]Magazine, error) {
	p := parameters{
		login:     session.login,
		password:  session.password,
		profileID: session.profileID,
	}

	resp, err := retry(ctx, "libraryService", p)

	if err != nil {
		return nil, errors.Wrap(err, "failed to download list of all magazines")
	}

	magazines := make(map[string]*Magazine)

	for _, publication := range resp.Library.LibraryPublications {
		pubID := publication.Publication.PubID
		mag, ok := magazines[pubID]

		if !ok {
			mag = &Magazine{ID: MagazineID(pubID), Title: publication.Publication.DisplayName}
			magazines[pubID] = mag
		}

		mag.Issues = append(mag.Issues, IssueID(publication.LibraryIssue.Issue.IssueID))
	}

	var res []Magazine

	for _, mag := range magazines {
		res = append(res, *mag)
	}

	return res, nil
}

// GetIssue downloads metadata for single magazine issue.
func (session *Session) GetIssue(ctx context.Context, magazine MagazineID, issue IssueID) (*Issue, error) {
	p := parameters{
		login:     session.login,
		password:  session.password,
		profileID: session.profileID,
		pubID:     magazine,
		issueID:   issue,
	}

	resp, err := retry(ctx, "issueData", p)

	if err != nil {
		return nil, errors.Wrapf(err, "failed to download metadata for magazine %s, issue %s", magazine, issue)
	}

	res := resp.IssuePackingList.SingleIssue
	password, err := decryptPdfPassword(res.TrackingCode.Init, res.TrackingCode.Value)

	if err != nil {
		return nil, errors.Wrapf(err, "failed to decrypt PDF password for magazine %s, issue %s", magazine, issue)
	}

	isssue := Issue{
		ID:        IssueID(res.IssueID),
		Title:     res.IssueTitle,
		Password:  password,
		PageCount: res.NumberOfPages,
		baseURL:   fmt.Sprintf("https://%s%s", res.HostName, res.IssueAssetDir),
	}

	return &isssue, nil
}

// GetURL returns download URL for a page.
func (issue Issue) GetURL(page int) (string, error) {
	if page < 0 || page >= issue.PageCount {
		return "", errors.Errorf("invalid page %d: must be between 0 and %d", page, issue.PageCount)
	}

	return fmt.Sprintf("%spage%d.pdf", issue.baseURL, page), nil
}
