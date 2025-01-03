from datetime import date

def save_file(joke:str) -> str:
    """
    Saves a text file using the textual content passed in the joke variable.
    Args:
        joke (str): The text content to be saved into the file. 
        
    Returns:
        Optional[str]: Returns 'success: file saved' on success, otherwise returns an error message.
    """
    file_name = "joke.txt"
    try:
        with open(file_name, "w") as f:
            f.write(joke)
        return "success: file saved"
    except Exception as e:
        return f"error: {str(e)}"


def get_flights(date_1:date, date_2:date) -> str:
    """ Returns the number of flights in a date interval  """
    import json
    from dateutil.parser import parse
    flights = {
        "flights": abs((parse(date_2) - parse(date_1)).days) 
    }
    return json.dumps(flights)


def my_cat_born_date() -> str:
    """ Returns my cat's born date """
    import datetime, random, json
    from dateutil.relativedelta import relativedelta
    
    # Calculate the date as ten years ago  
    ten_years_ago = datetime.date.today() - relativedelta(years=10) 
    
    cat_born_date = {
        "cat_born_date": ten_years_ago.strftime("%Y-%m-%d")
    }
    return json.dumps(cat_born_date)


def send_email(to:str, subject:str, body:str) -> str:
    """ Sends an email """
    import requests, json
    url = 'https://prod-18.swedencentral.logic.azure.com:443/workflows/4f7a19b041e04a9e8ea47303e1af503c/triggers/When_a_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_a_HTTP_request_is_received%2Frun&sv=1.0&sig=TX-eDahoU_QIEOjw9qOXjRyPNqA9s4IVkd0osbsyzzI'  

    headers = {
        'Content-Type': 'application/json'
    }

    data = {
        "to": to,
        "subject": subject,
        "body": body
    }
    response = {"response": str(requests.post(url, headers=headers, data=json.dumps(data)))}
    return json.dumps(response)