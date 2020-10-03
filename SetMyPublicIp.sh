#!/bin/bash

#helpful links:
#https://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html

_uri="https://jnhisz7ebh.execute-api.eu-west-1.amazonaws.com/api/hostedzone/Z6ZMEKJJ7H3SC/domain/marcelrienks.com"
_canonical_uri="%2Fapi%2Fhostedzone%2FZ6ZMEKJJ7H3SC%2Fdomain%2Fmarcelrienks.com"
_region="eu-west-1"
_service="execute-api"
_request="aws4_request"
_secret_key=""
_secret=""
#_time_stamp=$(date --utc +"%Y%m%dT%H%M%SZ")
_time_stamp="20200412T101415Z"
_date=$(date --utc +"%Y%m%d")

function main() {
  canonical_request=$(create_canonical_request)
  hashed_canonical_request=$(sha256_hash_in_hex "${canonical_request}")
  string_to_sign=$(create_string_to_sign "${hashed_canonical_request}")
  signing_key=$(derive_signing_key)
  signature=$(calculate_signature "${signing_key}" "${string_to_sign}")

  authorization_header="AWS4-HMAC-SHA256 Credential=${_secret_key}/${_date}/${_region}/${_service}/aws4_request, SignedHeaders=host;x-amz-date, Signature=${signature}"
  printf "curl --request PATCH --header \"Authorization: ${authorization_header}\" --header \"X-Amz-Date: ${_time_stamp}\" \"${_uri}\"\n"
  #AWS4-HMAC-SHA256 Credential=AKIAVKIFKRIEU7TFMEEI/20200412/eu-west-1/execute-api/aws4_request, SignedHeaders=host;x-amz-date, Signature=14952ff5d8057211f40716f4ad060c2cb15f8f0117898518424454695834b143
}

function create_canonical_request() {
  local http_request_method="PATCH"
  local canonical_uri=_canonical_uri
  local canonical_query_string=""
  local canonical_headers=""
  local signed_headers=""
  local request_payload=""

  local generated_hashed_request_payload=$(sha256_hash_in_hex "${request_payload}")

  printf "${http_request_method}\n${canonical_uri}\n${canonical_query_string}\n${canonical_headers}\n${signed_headers}\n${generated_hashed_request_payload}"
}

function create_string_to_sign() {
  local format_date=_date
  local hash_algorithm="AWS4-HMAC-SHA256"
  local time_stamp=_time_stamp
  local scope="${format_date}/${_region}/${_service}/${_request}"
  local supplied_hashed_canonical_request="$@"

  printf "${hash_algorithm}\n${time_stamp}\n${scope}\n${supplied_hashed_canonical_request}"
}

function derive_signing_key() {
  local secret="${_secret}"
  local date=$(hmac "AWS4${secret}" "${_format_date}")
  local region=$(hmac "${date}", "${_region}")
  local service=$(hmac "${region}", "${_service}")
  local signing=$(hmac "${service}", "${_request}")

  printf "${signing}"
}

function calculate_signature() {
  local signing="$1"
  local string="$2"
  shift 2
  printf "$string" | openssl dgst -binary -sha256 -mac HMAC -macopt "hexkey:$signing" | od -An -vtx1 | sed 's/[ \n]//g' | sed 'N;s/\n//'
}

function sha256_hash_in_hex() {
  local input="$@"
  printf "$input" | openssl dgst -binary -sha256 | od -An -vtx1 | sed 's/[ \n]//g' | sed 'N;s/\n//'
}

function hmac() {
  local key="$1"
  local value="$2"
  shift 2
  printf "$value" | openssl dgst -binary -sha256 -hmac "$key" | od -An -vtx1 | sed 's/[ \n]//g' | sed 'N;s/\n//'
}

main