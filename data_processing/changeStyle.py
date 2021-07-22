# -*- coding: utf-8 -*-
"""
Created on Tue May 26 12:08:05 2020

@author: ZiyaoHe
"""

import pandas as pd
import csv

file_name='Thang4.csv'
# Your working directory
#base_dir='C:/Users/ZiyaoHe/Documents/BirdImmersive/datasets/'
base_dir='E:/semester8/bachelor report/'
path=base_dir + file_name

individual_id=[]
lat=[]
lng=[]
height=[]
timestamp=[]
count=0#count Fehler
#csv_reader = csv.DictReader(path, delimiter=',')
#Go through every line in csv and check whether there is some empty vaule, if yes, discard them
with open(path, encoding='utf-8') as csv_file:
    csv_reader = csv.DictReader(csv_file, delimiter=',')
    for row in csv_reader:
        cur_time=row['timestamp']
        cur_lng=row['location-long']
        cur_lat=row['location-lat']
        cur_height=row['height-above-ellipsoid']
        cur_height=str(float(cur_height)-1000)
        
        individual_id.append('Thang Khaar')
        timestamp.append(cur_time)
        lng.append(cur_lng)
        lat.append(cur_lat)
        height.append(cur_height)
df = pd.DataFrame({
        'individual-local-identifier':individual_id,
        'timestamp':timestamp,
        'location-long':lng,
        'location-lat':lat,
        'height-above-ellipsoid':height
        })
##store whole data
df.to_csv(base_dir+"ThangKhaar4New.csv",sep=',',index=False,header=True)



