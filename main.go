package main

import (
	"bytes"
	"encoding/xml"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"os"
)

const baseURL = "https://services.zinio.com"

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

type zinioServiceResponse struct {
	XMLName        xml.Name `xml:"zinioServiceResponse"`
	ResponseStatus struct {
		Status      string `xml:"responseStatus>status"`
		ErrorDetail struct {
			Code    string `xml:"code"`
			Message string `xml:"message"`
		} `xml:"errorDetail"`
	} `xml:"responseStatus"`
	AuthenticateUserResponse struct {
		ProfileID string `xml:"profileId"`
	} `xml:"authenticateUserResponse"`
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

	req.RequestHeader.Authorization.Login = p.login
	req.RequestHeader.Authorization.Password = p.password

	req.RequestHeader.Device.ProfileID = p.profileID
	req.RequestHeader.Device.DeviceID = "A6B50079-BE65-44D6-A961-8A184AA81077"
	req.RequestHeader.Device.DeviceName = "iPhone"
	req.RequestHeader.Device.InstallationUUID = "84A8E36D-DF7F-4D7E-9824-F793AFB93207"
	req.RequestHeader.Device.PlatformDescription = "iPhone7,2"

	versions := []version{
		{"application", "20160314"},
		{"reader", "1.9"},
		{"storyBasedReader", "1.9"},
		{"security", "1.0"},
		{"shop", "1.0"},
		{"adSupport", "1.0"},
	}

	req.RequestHeader.Application.ApplicationName = "Zinio iReader"
	req.RequestHeader.Application.ApplicationVersion = "20160314"
	req.RequestHeader.Application.Versions = versions

	if p.pubID != "" && p.issueID != "" {
		req.LibraryIssueDataRequest = &libraryIssueDataRequest{
			PubID:   p.pubID,
			IssueID: p.issueID,
		}
	}

	return req
}

func main() {
	p := parameters{
		login:     os.Getenv("ZINIO_EMAIL"),
		password:  os.Getenv("ZINIO_PASSWORD"),
		profileID: "8751702194",
		pubID:     "373124878",
		issueID:   "416413259",
	}

	req := makeRequest(p)
	b, err := xml.Marshal(req)

	if err != nil {
		log.Fatal(err)
	}

	r := bytes.NewReader(b)
	resp, err := http.Post(baseURL+"/newsstandServices/issueData", "text/xml", r)

	if err != nil {
		log.Fatal(err)
	}

	defer resp.Body.Close()
	b, err = ioutil.ReadAll(resp.Body)

	if err != nil {
		log.Fatal(err)
	}

	fmt.Print(string(b))
}
