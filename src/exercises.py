import json


def get_exercises(event, context):
    body = {
        "message": "Go Gym 101!",
        "input": event
    }

    response = {
        "statusCode": 200,
        "body": json.dumps(body)
    }

    return response
