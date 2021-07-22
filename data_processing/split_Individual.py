# -*- coding: utf-8 -*-
"""
Created on Fri Nov 22 15:04:29 2019

@author: ZiyaoHe
"""
import os
import csv
#  Your working directory
base_dir='C:/Users/ZiyaoHe/Documents/BirdImmersion/white storks/'

with open(base_dir+'cleaned_data.csv', encoding='utf-8') as csv_file:
#    reader = csv.reader(csv_file)
#    headers = next(reader, None)
    csv_reader = csv.DictReader(csv_file, delimiter=',')
    fieldnames =  ['timestamp','location-long','location-lat','height-above-ellipsoid']#csv_reader.fieldnames
    #individual_id_unique=[]
    individual_id=[]
    
#    line_count = 0
    #individual_index_start=[]
    for row in csv_reader:
        if individual_id==[]:
            last_id=None
        else:
            last_id=individual_id[-1]
        cur_id=row['individual-local-identifier']
        individual_id.append(cur_id)
# In order to make the data consist with attribute:               
#                'individual-local-identifier':individual_id,
#                'timestamp':timestamp,
#                'location-long':lng,
#                'location-lat':lat,
#                'height-above-ellipsoid':height
                #the id of the row
        
        if last_id!=cur_id:
            newpath = base_dir+cur_id 
            if not os.path.exists(newpath):
                   os.makedirs(newpath)
            csvfile_write = open(newpath+'/'+cur_id+'.csv', 'w', newline='')
            writer = csv.DictWriter(csvfile_write,fieldnames=fieldnames)
            writer.writeheader()
        else:
            
            cur_time=row['timestamp']
            cur_lng=row['location-long']
            cur_lat=row['location-lat']
            cur_height=row['height-above-ellipsoid']
            row_dict={#'individual-local-identifier':cur_id,
                      'timestamp':cur_time,
                      'location-long':cur_lng,
                      'location-lat':cur_lat,
                      'height-above-ellipsoid':cur_height
                    }
            writer.writerow(row_dict)

                
        
        
    