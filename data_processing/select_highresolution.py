# -*- coding: utf-8 -*-
"""
Created on Thu Nov 28 10:56:44 2019
select the indices of high resolution data points,
outcome:= the list of peroid 
for example: outcome=[[0,1],[3,4,5]], this contains 2 period: row 0, row 1 and row3,row4 , row5 
@author: ZiyaoHe
"""

import pandas as pd
#import os
import csv

id_name='Chompy'# choose this for Example
# Your working directory
base_dir='C:/Users/ZiyaoHe/Documents/BirdImmersive/white storks/'+id_name+'/'

filePath=base_dir+id_name+'.csv'

Time_Diff_Fliter = 10# 10 seconds each point

df = pd.read_csv(filePath)
timestamp=df['timestamp']
len_time=len(timestamp)
high_resolution_list=[]
high_resolution_peroid=[]

'''
Decide whether the year is leap Year
'''
def leapYear(year):
    if year%4==0:
        if year%100!=0:
            return 366
    if year%400==0:
            return 366
    return False
# How many days in a month
def monthDays(M,year):
    if M==2:
        if leapYear(year):
            return 29
        else:
            return 28
    elif M in [1,3,5,7,8,10,12]:
        return 31
    else:
        return 30

for i in range(0,len_time-1):
    
    cur_timestamp=timestamp[i]
    next_timestamp=timestamp[i+1]
    #print(cur_timestamp)
#    print(next_timestamp)
    #split timestamp as string
    # Example: 2007-10-16 05:20:22.998
    cur_timestamp=cur_timestamp.split(' ')
    next_timestamp=next_timestamp.split(' ')
    #Example: 2007-10-16
    cur_date=cur_timestamp[0]
    next_date=next_timestamp[0]
    #['2007','10','16']
    cur_YY_MM_DD=cur_date.split('-')
    #2007
    cur_Year=cur_YY_MM_DD[0]
    cur_Year=int(cur_Year)
    #10
    cur_Mon=cur_YY_MM_DD[1]
    cur_Mon=int(cur_Mon)
    #16
    cur_Day=cur_YY_MM_DD[2]
    cur_Day=int(cur_Day)
    
    next_YY_MM_DD=next_date.split('-')
    #2007
    next_Year=next_YY_MM_DD[0]
    next_Year=int(next_Year)
    #10
    next_Mon=next_YY_MM_DD[1]
    next_Mon=int(next_Mon)
    #16
    next_Day=next_YY_MM_DD[2]
    next_Day=int(next_Day)
    
    #Example: 05:20:22.998
    cur_time=cur_timestamp[1]
    
    next_time=next_timestamp[1]
    #Example:['05','20','22.998']
    cur_H_M_S=cur_time.split(':')
    cur_H=int(cur_H_M_S[0])
    cur_M=int(cur_H_M_S[1])
    cur_S=float(cur_H_M_S[2])
    
    next_H_M_S=next_time.split(':')
    next_H=int(next_H_M_S[0])
    next_M=int(next_H_M_S[1])
    next_S=float(next_H_M_S[2])
# We don't need to care about the leap year and how any days in a Month. Doch!
#    cur_time_insecond=(cur_Year-1)*365*24*60*60+(cur_Mon-1)*30*24*60*60+(cur_Day-1)*24*60*60+cur_H*60*60+cur_M*60+cur_S
#    next_time_insecond=(next_Year-1)*365*24*60*60+(next_Mon-1)*30*24*60*60+(next_Day-1)*24*60*60+next_H*60*60+next_M*60+next_S
    if next_Year-cur_Year==1:
        if cur_Mon==12 and next_Mon==1:
            if cur_Day==31 and next_Day==1:
                diff=24*60*60+(next_H-cur_H)*60*60+(next_M-cur_M)*60+next_S-cur_S
            else:
                diff=10000#set a big difference(because we are not interested in that time skip)
        else:
            diff=100000#set a big difference(because we are not interested in that time skip)
    elif next_Year-cur_Year==0:
        diff = (next_Mon-cur_Mon)*monthDays(cur_Mon,cur_Year)*24*60*60+(next_Day-cur_Day)*24*60*60+(next_H-cur_H)*60*60+(next_M-cur_M)*60+next_S-cur_S
    else:
        diff=1000000
    # print(diff)  
#    diff_l.append(diff)
    if abs(diff)<=Time_Diff_Fliter:
#         print(diff)
         high_resolution_peroid.append(i)#add the row number into a list
         if (i==(len_time-2)):#If we add the second last time , then add the last timestamp
#             print('selected')
             high_resolution_peroid.append(i+1)
             high_resolution_list.append(high_resolution_peroid)
    else:
        high_resolution_peroid.append(i)
        high_resolution_list.append(high_resolution_peroid)
        high_resolution_peroid=[]
#high_resolution_list=list(set(high_resolution_list))# make it unique
outcome=[]
for i in high_resolution_list:
    if len(i)>1:
        outcome.append(i)
print(outcome)

#############################################
# now we have "outcome" as a list of rows for each highresolution period
#############################################

count=1#count the number of period
for item in outcome:
    csv_file = open(filePath, encoding='utf-8') 
    csv_reader = csv.DictReader(csv_file, delimiter=',')
    fieldnames = csv_reader.fieldnames
    csvfile_write = open(base_dir+'/'+str(count)+'.csv', 'w')
    writer = csv.DictWriter(csvfile_write,fieldnames=fieldnames)
    writer.writeheader()
    count=count+1
    rowcounter=0
    for row in csv_reader:
        for row_index in item:
            
            if rowcounter==row_index:
                    #(row)
                    writer.writerow(row)
        rowcounter=rowcounter+1














