#!/bin/bash

_canonical_uri="%2Fapi%2Fhostedzone%2FZ6ZMEKJJ7H3SC%2Fdomain%2Fmarcelrienks.com"
_region="eu-west-1"
_date=date+"%Y%m%d"

#tested
function create_canonical_request() {
  http_request_method="PATCH"
  canonical_uri=_canonical_uri
  canonical_query_string=""
  canonical_headers=""
  signed_headers=""
  request_payload=""

  generated_hashed_request_payload=$(sha256_hash_in_hex "${request_payload}")

  generated_canonical_request="${http_request_method}\n${canonical_uri}\n${canonical_query_string}\n${canonical_headers}\n${signed_headers}\n${generated_hashed_request_payload}"
  printf "${generated_canonical_request}"
}

function create_string_to_sign() {
  _format_date=$(format_date)

  hash_algorithm="AWS4-HMAC-SHA256"
  time_stamp=$(iso_format_date)
  scope="${_format_date}/${_region}/apigateway/aws4_request"
  supplied_hashed_canonical_request="$@"

  generated_string_to_sign="${hash_algorithm}\n${time_stamp}\n${scope}\n${supplied_hashed_canonical_request}"
  printf "${generated_string_to_sign}"
}

#tested
function sha256_hash_in_hex() {
  a="$@"
  printf "$a" | openssl dgst -binary -sha256 | od -An -vtx1 | sed 's/[ \n]//g' | sed 'N;s/\n//'
}

#tested
function iso_format_date() {
  printf $(date +"%Y%m%dT%H%M%SZ")
}

#tested
function format_date() {
  printf $(date +"%Y%m%d")
}

canonical_request=$(create_canonical_request)
hashed_canonical_request=$(sha256_hash_in_hex "${canonical_request}")
string_to_sign=$(create_string_to_sign "${hashed_canonical_request}")
printf "$string_to_sign"