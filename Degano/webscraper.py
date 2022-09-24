from bs4 import BeautifulSoup
import requests

def get_fuel_data():

    html_text = requests.get('https://gas.didnt.work/?country=lt&brand=&city=Vilnius').text
    soup = BeautifulSoup(html_text, 'lxml')

    data_file = open("data.txt", "w", encoding="utf-8")

    tbody = soup.find('tbody')
    trs = tbody.findAll('tr')
    for tr in trs:
        tr.find('span').decompose()
        tr.find('br').decompose()
        address = tr.find('small').text
        tr.find('small').decompose()
        gas_station_name = tr.find('td').text[2:]
        diesel = tr.find('td').findNext('td').text
        petrol95 = tr.find('td').findNext('td').findNext('td').text
        petrol98 = tr.find('td').findNext('td').findNext('td').findNext('td').text
        LPG = tr.find('td').findNext('td').findNext('td').findNext('td').findNext('td').text
        if diesel and petrol95 != "-":
            data_file.write(f"Name: {gas_station_name.strip()}\nAddress: {address}\nDiesel: {diesel}\n95: {petrol95}\n98: {petrol98}\nLPG: {LPG}\n")

    data_file.close()

if __name__ == '__main__':
    get_fuel_data()