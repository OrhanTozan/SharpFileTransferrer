# SharpFileTransferrer
Send and receive files through the internet!

# How to use it in your project
1. Download the latest .dll from https://github.com/NahroTo/SharpFileTransferrer/releases
2. In Visual studio, go to References right click and select add reference
3. Press the browse button and select the dll file you just downloaded

# Instructions
  ```c#
  // CLIENT side code
  
  // send one file..
  SharpClient.SendFile(192.168.178.16, 49181, @"C:\potatoes\banana.txt");
  
  // or more than one file
  string[] files = {"C:\potatoes\banana.txt",
    @"C:\chickens\cow.txt",
    @"C:\banana.jpg"};
  SharpClient.SendFiles(192.168.178.16, 49181, files);
  
  // SERVER side code
  
  SharpServer.ReceiveFiles(49181, @"C:\destinationfolder");
  ```
