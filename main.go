package main

import (
	"bytes"
	"context"
	"encoding/xml"
	"errors"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"time"
)

const (
	baseURL     = "https://services.zinio.com/newsstandServices/"
	httpTimeout = 30 * time.Second

	deviceID         = "A6B50079-BE65-44D6-A961-8A184AA81077"
	installationUUID = "84A8E36D-DF7F-4D7E-9824-F793AFB93207"
)

type version struct {
	Key   string `xml:"key,attr"`
	Value string `xml:"value,attr"`
}

type libraryIssueDataRequest struct {
	PubID   string `xml:"pubId"`
	IssueID string `xml:"issueId"`
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

type issuePackingList struct {
	SingleIssue []struct {
		PubID         string `xml:"pubId"`
		IssueTitle    string `xml:"issueTitle"`
		IssueID       string `xml:"issueId"`
		HostName      string `xml:"hostName"`
		IssueAssetDir string `xml:"issueAssetDir"`
		TrackingCode  struct {
			Init  string `xml:"init,attr"`
			Value string `xml:",chardata"`
		} `xml:"trackingCode"`
		NumberOfPages string `xml:"numberOfPages"`
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
	IssuePackingList         issuePackingList         `xml:"issuePackingList"`
}

type parameters struct {
	login     string
	password  string
	profileID string
	pubID     string
	issueID   string
}

func makeRequest(p parameters) zinioServiceRequest {
	var req zinioServiceRequest
	h := &req.RequestHeader

	h.Authorization.Login = p.login
	h.Authorization.Password = p.password

	h.Device.ProfileID = p.profileID
	h.Device.DeviceID = deviceID
	h.Device.DeviceName = "iPhone"
	h.Device.InstallationUUID = installationUUID
	h.Device.PlatformDescription = "iPhone7,2"

	versions := []version{
		{"application", "20160314"},
		{"reader", "1.9"},
		{"storyBasedReader", "1.9"},
		{"security", "1.0"},
		{"shop", "1.0"},
		{"adSupport", "1.0"},
	}

	h.Application.ApplicationName = "Zinio iReader"
	h.Application.ApplicationVersion = "20160314"
	h.Application.Versions = versions

	if p.pubID != "" && p.issueID != "" {
		req.LibraryIssueDataRequest = &libraryIssueDataRequest{
			PubID:   p.pubID,
			IssueID: p.issueID,
		}
	}

	return req
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
	resp, err := http.Post(baseURL+query, "text/xml", r)

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

func authenticateUser(ctx context.Context, login, password string) (*authenticateUserResponse, error) {
	p := parameters{
		login:    login,
		password: password,
	}

	resp, err := post(ctx, "authenticateUser", p)

	if err != nil {
		return nil, err
	}

	return &resp.AuthenticateUserResponse, nil
}

func issueData(ctx context.Context, login, password, profileID, pubID, issueID string) (*issuePackingList, error) {
	p := parameters{
		login:     login,
		password:  password,
		profileID: profileID,
		pubID:     pubID,
		issueID:   issueID,
	}

	resp, err := post(ctx, "issueData", p)

	if err != nil {
		return nil, err
	}

	return &resp.IssuePackingList, nil
}

func main() {
	ctx := context.Background()

	login := os.Getenv("ZINIO_EMAIL")
	password := os.Getenv("ZINIO_PASSWORD")

	auth, err := authenticateUser(ctx, login, password)

	if err != nil {
		log.Fatal(err)
	}

	issue, err := issueData(ctx, login, password, auth.ProfileID, "373124878", "416413259")

	if err != nil {
		log.Fatal(err)
	}

	code := issue.SingleIssue[0].TrackingCode
	iv := code.Init
	ciphertext := code.Value

	fmt.Println(iv)
	fmt.Println(ciphertext)
}
