Execute the function using this input syntax:

{
  "Region": "<AWS region code>",
  "Bucketname": "<AWS S3 bucket name>",
  "Source": "<Path to JSON file in the bucket>",
  "Fieldname": "<JSON field that will be used for splitting the array>",
  "AddContext": <true or false. If true, it will add metadata to the resulting json files>,
  "DefaultCategory": "<Name of default category>",
"ResultPathPrefix": "<AWS S3 prefix for the result files>",
"FilenameSyntax": "<syntax for the filename of the resulting files. {category} token can be used and it will be replaced>"
}
