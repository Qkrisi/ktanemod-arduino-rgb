int redpin = 0;
int greenpin = 0;
int bluepin = 0;
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
      
    redpin=arr[0];
    greenpin=arr[1];
    bluepin=arr[2];
    lastRValue=arr[3];
    lastGValue=arr[4];
    lastBValue=arr[5];
  }
  
  if(redpin>0 && greenpin>0 && bluepin>0){
    pinMode (redpin, OUTPUT);
    pinMode (greenpin, OUTPUT);
    pinMode (bluepin, OUTPUT);
    analogWrite(redpin,lastRValue);
    analogWrite(greenpin,lastGValue);
    analogWrite(bluepin,lastBValue);
  }
}
