#!/bin/bash

function create_canonical_request() {
  httpRequestMethod = "PATCH"
  canonicalUri = "%2Fapi%2Fhostedzone%2FZ6ZMEKJJ7H3SC%2Fdomain%2Fmarcelrienks.com"
  canonicalQueryString = ""
  canonicalHeaders = ""
  signedHeaders = ""
  requestPayload = ""

  hashedRequestPayload = $(sha256_hash_in_hex "${requestPayload}")

  canonicalRequestContent = "${httpRequestMethod}\n${canonicalUri}\n${canonicalQueryString}\n${canonicalHeaders}\n${signedHeaders}\n${hashedRequestPayload}"
  canonicalRequest = $(sha256_hash_in_hex "${canonicalRequestContent}")

  printf "$canonicalRequest" 
}

create_canonical_request

sha256_hash_in_hex(){
  a="$@"
  printf "$a" | openssl dgst -binary -sha256 | od -An -vtx1 | sed 's/[ \n]//g' | sed 'N;s/\n//'
}