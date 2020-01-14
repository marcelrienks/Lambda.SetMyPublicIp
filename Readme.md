[![Build Status](https://dev.azure.com/marcelrienks/SetMyPublicIp/_apis/build/status/marcelrienks.Lambda.SetMyPublicIp?branchName=master)](https://dev.azure.com/marcelrienks/SetMyPublicIp/_build/latest?definitionId=15&branchName=master)

[![Build Status](https://github.com/marcelrienks/Lambda.SetMyPublicIp/workflows/dotnet-core/badge.svg)](https://github.com/marcelrienks/Lambda.SetMyPublicIp/actions?query=workflow%3Adotnet-core)

# SetMyPublicIp
This is a DDNS (Dynamic DNS) solution implemented using AWS Route 53 and AWS Lambda.

The goal is to have some sort of recurring script, which will call an API function through AWS API Gateway, executing this Lambda function, which updates the record set of a domain registered in AWS Route 53 to point to the public IP address of the network that executed the recurring script.

**Note:** The same solution could be achieved by simply making an AWS SDK call from some sort of recurring script, that will update the record set of a domain registered in AWS Route 53, removing the need for an AWS Lambda.  
However the purpose of this project was to learn how to use AWS Lambda to solve the problem.

## WIP (Work In Progress)
This is still a work in progress

## Usage
Complete this
### Route 53 Domain configuration
Complete this
### API Gateway configuration
Complete this
### AWS Lambda setup
Complete this
### Script Example:
Complete this
