# -*- coding: utf-8 -*-
"""
Created on Mon Nov 25 12:13:04 2019

Do data cleaning currently we only use
timestamp,location-long,location-lat,
individual-local-identifier, and
height-above-ellipsoid
for Oilbirds dataset

@author: ZiyaoHe
"""

import pandas as pd
import csv

file_name='Fall migration of white storks in 2014.csv'
# Your working directory
#base_dir='C:/Users/ZiyaoHe/Documents/BirdImmersive/datasets/'
base_dir='E:/semester8/bachelor report/test/'
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
        if (cur_lat!="") and (cur_lng!="") and (cur_height!=""):
            # according to the id name you can also switch to  
            #individual_id.append(row['individual-local-identifier'])
            individual_id.append(row['individual-local-identifier'].split(' ')[0])#for white storks dataset
            lat.append(cur_lat)
            lng.append(cur_lng)
            timestamp.append(cur_time)
            height.append(cur_height)
        else:
            count=count+1
        

df = pd.DataFrame({
        'individual-local-identifier':individual_id,
        'timestamp':timestamp,
        'location-long':lng,
        'location-lat':lat,
        'height-above-ellipsoid':height
        })
##store whole data
df.to_csv(base_dir+"cleaned_data.csv",sep=',',index=False,header=True)



