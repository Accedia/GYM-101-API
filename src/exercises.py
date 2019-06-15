import json
import boto3
from boto3.dynamodb.conditions import Key


def get_exercises(event, context):

    queryStringParameters = event.get('queryStringParameters', {})
    equipment = queryStringParameters['equipment']
    print(equipment)

    dynamodb = boto3.resource('dynamodb')
    table = dynamodb.Table('gym101-dev')

    response = table.query(
        KeyConditionExpression=Key('equipment').eq(equipment)
    )
    items = response['Items']

    response = {
        "statusCode": 200,
        "body": json.dumps(items)
    }

    return response
