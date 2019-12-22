int redpin = -1;
int greenpin = -1;
int bluepin = -1;
int lastRValue = 0;
int lastGValue = 0;
int lastBValue = 0;

void setup(){
    Serial.begin(9600);
}

void loop(){
  if (Serial.available() > 0) {
 
    String s = Serial.readString(); 
  
    int n = s.length(); 
  
    char str[n + 1]; 
  
    strcpy(str, s.c_str()); 
   
    const size_t bufferSize = 6;
    int arr[bufferSize];
  
    char *p = strtok(str, " ");
    size_t index = 0;
  
    while (p != nullptr && index < bufferSize) {
        arr[index++] = atoi(p);
        p = strtok(NULL, " ");
    }
      
    redpin=arr[1];
    greenpin=arr[0];
    bluepin=arr[2];
    lastRValue=arr[3] % 256;
    lastGValue=arr[4] % 256;
    lastBValue=arr[5] % 256;
  }
  
  if(redpin>-1 && greenpin>-1 && bluepin>-1){
    pinMode (redpin, OUTPUT);
    pinMode (greenpin, OUTPUT);
    pinMode (bluepin, OUTPUT);
    analogWrite(redpin,lastRValue);
    analogWrite(greenpin,lastGValue);
    analogWrite(bluepin,lastBValue);
  }
}
