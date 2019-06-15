Deployment instructions:
1. Deply solution - sls deploy -v
2. Load data in DB - aws dynamodb batch-write-item --table-name gym101-dev --request-items file://db.json