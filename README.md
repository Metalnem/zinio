# Zinio DRM removal [![Build Status](https://travis-ci.org/Metalnem/zinio.svg?branch=master)](https://travis-ci.org/Metalnem/zinio) [![Go Report Card](https://goreportcard.com/badge/github.com/metalnem/zinio)](https://goreportcard.com/report/github.com/metalnem/zinio) [![license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://raw.githubusercontent.com/metalnem/zinio/master/LICENSE)

This command line tool removes DRM from magazines in your digital Zinio library.
For more details visit my blog post
[Removing Zinio DRM](https://mijailovic.net/2017/06/06/removing-zinio-drm/).

## Frequently Asked Questions

#### Do I have to download the whole library every time?

No, you don't—this tool skips issues that you have
already downloaded. It will only download the magazines
that are not present in your download folder.

#### Can I download a specific magazine issue?

I agree that this would be a nice feature, but I will
not implement it, mostly because it would be complicated
to use from the command line. Again, you don't have
to download the whole library each time, so this feature
is not essential in my opinion.

## Downloads

[Windows](https://github.com/Metalnem/zinio/releases/download/v1.1.0/zinio-win64-1.1.0.zip)  
[Mac OS X](https://github.com/Metalnem/zinio/releases/download/v1.1.0/zinio-darwin64-1.1.0.zip)  
[Linux](https://github.com/Metalnem/zinio/releases/download/v1.1.0/zinio-linux64-1.1.0.zip)


## Usage

```
$ ./zinio
Usage of zinio:
  -email string
    	Account email
  -password string
    	Account password
```

## Example

```
$ ./zinio -email user@example.org -password secret123 
```
