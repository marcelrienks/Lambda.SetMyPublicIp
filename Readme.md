[![Build Status](https://github.com/marcelrienks/Lambda.SetMyPublicIp/workflows/dotnet-core/badge.svg)](https://github.com/marcelrienks/Lambda.SetMyPublicIp/actions?query=workflow%3Adotnet-core)

# SetMyPublicIp
This is a DDNS (Dynamic DNS) solution implemented using AWS Route 53 and AWS Lambda.

The goal is to have some sort of recurring script, which will call an API function through AWS API Gateway, executing this Lambda function, which updates the record set of a domain registered in AWS Route 53 to point to the public IP address of the network that executed the recurring script.

**Note:** The same solution could be achieved by simply making an AWS SDK call from some sort of recurring script, that will update the record set of a domain registered in AWS Route 53, removing the need for an AWS Lambda.  
However the purpose of this project was to learn how to use AWS Lambda to solve the problem.

## WIP (Work In Progress)
This is still a work in progress
### Todo:
* Complete Cloud Formation script 'AWS-CloudFormation.json'
  * Remove prefix 'Pre-' from the names of resources once the script is working
* Complete bash script 'SetMyPublicIp.sh'
* Complete readme file 'README.md'

## Deploy
Complete this...
1. Ensure that you have a hosted zone on a domain set up in Route 53 of AWS
2. Use 'Lambda tools' to package the project into a zip file
3. Upload the zip to an S3 bucket in the same region that you intend to deploy the Cloud Formation Stack to
4. Using the 'AWS-CloudFormation.json' to create and deploy a CloudFormation stack  
4.1. Reference the Hosted Zone from point 1 above, that you would like to set the public ip recordset for  
4.2. Reference the zip file name that was uploaded to S3  
4.3. Reference the name of the S3 bucket that the zip file was uploaded to
### AWS Lambda tools
#### Setup
1. `dotnet tool install -g Amazon.Lambda.Tools`
2. `dotnet tool update -g Amazon.Lambda.Tools`
#### Package
Package the project into a zip file that can be used to manually opload to an S3 bucket.  
This method can them be used along with a Cloud Formation script, for deploying a full stack.
3. from the 'Lambda.SetMyPublicIp' project folder  
`dotnet lambda package`
### Cloud Formation script
There is a Cloud Formation script at the solution layer called `AWS-CloudFormation.json`.  
This script can be used, along with the packaged source created by using AWS Lambda tools from above, for deploying a full stack.
### Script Example:
Complete this
