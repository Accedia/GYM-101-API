import json
import os
import boto3
from boto3.dynamodb.conditions import Key


def get_exercises(event, context):

    queryStringParameters = event.get('queryStringParameters', {})
    equipment = queryStringParameters['equipment']
    print(equipment)

    table_name = os.environ['DYNAMODB_TABLE']
    dynamodb = boto3.resource('dynamodb')
    table = dynamodb.Table(table_name)

    response = table.query(
        KeyConditionExpression=Key('equipment').eq(equipment)
    )
    items = response['Items']

    response = {
        "statusCode": 200,
        "body": json.dumps(items)
    }

    return response
