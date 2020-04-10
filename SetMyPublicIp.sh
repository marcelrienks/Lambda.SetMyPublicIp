#!/bin/bash

_canonical_uri="%2Fapi%2Fhostedzone%2FZ6ZMEKJJ7H3SC%2Fdomain%2Fmarcelrienks.com"
_region="eu-west-1"
_service="execute-api"
_request="aws4_request"
_secret_key=""
_secret=""

function create_canonical_request() {
  http_request_method="PATCH"
  canonical_uri=_canonical_uri
  canonical_query_string=""
  canonical_headers=""
  signed_headers=""
  request_payload=""

  generated_hashed_request_payload=$(sha256_hash_in_hex "${request_payload}")

  printf "${http_request_method}\n${canonical_uri}\n${canonical_query_string}\n${canonical_headers}\n${signed_headers}\n${generated_hashed_request_payload}"
}

function create_string_to_sign() {
  _format_date=$(format_date)

  hash_algorithm="AWS4-HMAC-SHA256"
  time_stamp=$(iso_format_date)
  scope="${_format_date}/${_region}/${_service}/${_request}"
  supplied_hashed_canonical_request="$@"

  printf "${hash_algorithm}\n${time_stamp}\n${scope}\n${supplied_hashed_canonical_request}"
}

function derive_signing_key() {
  secret="${_secret}"
  date=$(hmac "AWS4${secret}" "${_format_date}")
  region=$(hmac "${date}", "${_region}")
  service=$(hmac "${region}", "${_service}")
  signing=$(hmac "${service}", "${_request}")

  printf "${signing}"
}

function calculate_signature() {
  signing="$1"
  string="$2"
  shift 2
  printf "$string" | openssl dgst -binary -sha256 -mac HMAC -macopt "hexkey:$signing" | od -An -vtx1 | sed 's/[ \n]//g' | sed 'N;s/\n//'
}

function sha256_hash_in_hex() {
  input="$@"
  printf "$input" | openssl dgst -binary -sha256 | od -An -vtx1 | sed 's/[ \n]//g' | sed 'N;s/\n//'
}

function hmac() {
  key="$1"
  value="$2"
  shift 2
  printf "$value" | openssl dgst -binary -sha256 -hmac "$key" | od -An -vtx1 | sed 's/[ \n]//g' | sed 'N;s/\n//'
}

function iso_format_date() {
  printf $(date +"%Y%m%dT%H%M%SZ")
}

function format_date() {
  printf $(date +"%Y%m%d")
}

canonical_request=$(create_canonical_request)
hashed_canonical_request=$(sha256_hash_in_hex "${canonical_request}")
string_to_sign=$(create_string_to_sign "${hashed_canonical_request}")
signing_key=$(derive_signing_key)
signature=$(calculate_signature "${signing_key}" "${string_to_sign}")
date=$(format_date)

authorization_header="AWS4-HMAC-SHA256 Credential=${_secret_key}/${date}/${_region}/${_service}/aws4_request, SignedHeaders=host;x-amz-date, Signature=${signature}"

#AWS4-HMAC-SHA256 Credential=AKIAVKIFKRIEU7TFMEEI/20200410/eu-west-1/execute-api/aws4_request, SignedHeaders=host;x-amz-date, Signature=4bca10e7885a56f056b44f63822bae1d963b66e99a4cc3acbcbee496b4b6fe47
printf "$authorization_header\n"