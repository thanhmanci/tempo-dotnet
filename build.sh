docker build -t thanhmanci/test:tempodotnet_v06 -f TempoDotNet/Dockerfile .
docker build -t thanhmanci/test:logservice_v06 -f LogService/Dockerfile .
docker push thanhmanci/test:tempodotnet_v06
docker push thanhmanci/test:logservice_v06
