from bs4 import BeautifulSoup
import requests


def get_fuel_data():

    html_text = requests.get(
        'https://gas.didnt.work/?country=lt&brand=&city=Vilnius').text
    soup = BeautifulSoup(html_text, 'lxml')

    data_file = open("data.txt", "w", encoding="utf-8")

    gas_station_brands = ["Alauša", "Baltic Petroleum", "Circle K", "EMSI",
                          "Ecoil", "Jozita", "Kvistija", "Neste", "Orlen", "Skulas", "Stateta", "Takuras", "Viada"]

    tbody = soup.find('tbody')
    trs = tbody.findAll('tr')
    for tr in trs:
        tr.find('span').decompose()
        tr.find('br').decompose()
        address = tr.find('small').text
        tr.find('small').decompose()

        # Getting the coordinates of the given address using Google Maps API
        address_for_url = address
        address_for_url.replace(' ', '+')
        url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + \
            address_for_url + "&key=AIzaSyBbkz9JBShE8JYmYFoU2XG-jqIigrR4jyg"
        response = requests.get(url)
        json = response.json()
        latitude = json['results'][0]['geometry']['location']['lat']
        longitude = json['results'][0]['geometry']['location']['lng']

        gas_station_name = tr.find('td').text[2:]

        # Getting only the gas station brand from the name
        for brand in gas_station_brands:
            if brand in gas_station_name:
                gas_station_brand = brand
                break

        diesel = tr.find('td').findNext('td').text
        petrol95 = tr.find('td').findNext('td').findNext('td').text
        petrol98 = tr.find('td').findNext(
            'td').findNext('td').findNext('td').text
        LPG = tr.find('td').findNext('td').findNext(
            'td').findNext('td').findNext('td').text
        if diesel and petrol95 != "-" and gas_station_name.strip() != "A. Lingės degalinė":
            data_file.write(
                f"{gas_station_brand}\n{gas_station_name.strip()}\n{address}\n{latitude}\n{longitude}\n{diesel}\n{petrol95}\n{petrol98}\n{LPG}\n")

    data_file.close()


if __name__ == '__main__':
    get_fuel_data()
