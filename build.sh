docker build -t thanhmanci/test:tempodotnet_v11 -f TempoDotNet/Dockerfile .
docker build -t thanhmanci/test:logservice_v11 -f LogService/Dockerfile .
docker push thanhmanci/test:tempodotnet_v11
docker push thanhmanci/test:logservice_v11
