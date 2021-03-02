cd ../Cryptography.Cicd
./build.sh
cd ../Cryptography.Autotests
cp ../Cryptography.DemoApplication/bin/Debug/net5.0/Cryptography.DemoApplication.exe ./Cryptography.DemoApplication.exe
python ./demontraiting_app_tests.py ./Cryptography.DemoApplication.exe