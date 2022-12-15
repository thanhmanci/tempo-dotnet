#!/bin/bash
counter=$[counter+1]
echo "==========================================="
echo $counter
echo "==========================================="

cd /mnt/c/Users/thanh.ma/source/repos/TempoDotNet
docker build -t "thanhmanci/testt:app1_v_$counter" -f App1/Dockerfile .
docker build -t "thanhmanci/testt:app2_v_$counter" -f App2/Dockerfile .
docker build -t "thanhmanci/testt:app3_v_$counter" -f App3/Dockerfile .
docker build -t "thanhmanci/testt:app4_v_$counter" -f App4/Dockerfile .

docker push "thanhmanci/testt:app1_v_$counter"
docker push "thanhmanci/testt:app2_v_$counter"
docker push "thanhmanci/testt:app3_v_$counter"
docker push "thanhmanci/testt:app4_v_$counter"
cd 
cd linhtinh/