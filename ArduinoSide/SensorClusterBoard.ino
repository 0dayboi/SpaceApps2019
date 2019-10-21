//Written By member NartGashi for Space Apps Challenge 2019 project SpaceMan. Date Of Creation 20/10/2019
#include <SoftwareSerial.h>
#include <SPI.h>
#define LOG_PERIOD 5000 //Logging period in milliseconds, recommended value 15000-60000.
#define MAX_PERIOD 60000 //Maximum logging period

//Radioactivity Variables
volatile unsigned long counts = 0;                       // GM Tube events
unsigned long cpm = 0;                                   // CPM
const unsigned int multiplier = MAX_PERIOD / LOG_PERIOD; // Calculates/stores CPM
unsigned long previousMillis;                            // Time measurement
unsigned long previousMillis2;   
const int pin = 3;
//EndRadioactivity Variables

//Sensor Pinouts
   int UV_1 = 35; //ultra violet lights
   int UV_2 = 34;
   int UV_3 = 37;
   
   int white_light1 = 42;
   int white_light2 = 46;

   int ElectroMagnet = 30;
   int KufitarjaMagnet = 27;
    
   int IR_Silica = A6;
//End Sensor Pinouts

//Sensor Values
   int value_kufitarja = 0;
   int value_IR = 0;

//SensorValues
   String BtData = "";
   bool SampleAbortion = false;
   bool PossibilityOfSilicia = false;
   bool PossibilityOfMagneticProperties = false;
   
//Misc variables

void tube_impulse() { // Captures count of events from Geiger counter board
  counts++;
}

//End Misc Variables

void setup() {
  // put your setup code here, to run once:
  Serial3.begin(9600);
  Serial.begin(9600);
  pinMode(pin, INPUT);                                      
  interrupts();                                                      
  attachInterrupt(digitalPinToInterrupt(pin), tube_impulse, FALLING); 
//Defining the pin modes
  pinMode(UV_1, OUTPUT);
    pinMode(UV_2, OUTPUT);
      pinMode(UV_3, OUTPUT);
  pinMode(white_light1, OUTPUT);
    pinMode(white_light2, OUTPUT);
  pinMode(ElectroMagnet, OUTPUT);
  pinMode(KufitarjaMagnet, INPUT);
//End Defining the pin modes
  //CreateSample();
     
}

void loop() {
  // put your main code here, to run repeatedly:
  while (Serial3.available() > 0) {
    Serial.println("WOW!");
  
     BtData = Serial3.read();
       Serial.println(BtData);
     CommandTaken(BtData);
     
  }
value_kufitarja = digitalRead(KufitarjaMagnet);
  value_IR = analogRead(IR_Silica);
  unsigned long currentMillis = millis();
   if(currentMillis - previousMillis2 > 3000) {
    previousMillis = currentMillis;
    Serial3.println("#" + String(value_kufitarja) + "%" + String(value_IR) + "%");
   }
  
  //RadMeter();
     
}


void RadMeter() {
 unsigned long currentMillis = millis();
   if(currentMillis - previousMillis > LOG_PERIOD) {
    previousMillis = currentMillis;
    cpm = counts * multiplier;
    Serial.println("CPM : " + cpm);
    counts = 0;
    Serial3.println("%" + String(cpm) + "%");
   }
}

void CreateSample() {
  //Radiation.
   RadMeter();
   
   SetWhiteLights(true);   delay(500);   SetWhiteLights(false);   delay(50); SetWhiteLights(true);  delay(50);  SetWhiteLights(false);  //Indicate Start
     RadMeter();
   SetUVLights(true);
     RadMeter();
   delay(6000);
   Serial3.println(".1.");
   SetUVLights(false);
     RadMeter();
     SetWhiteLights(true); 
     delay(5000);
       Serial3.println(".2.");
       RadMeter();
   SetWhiteLights(false);  delay(50);
    SetWhiteLights(true);  delay(50);  SetWhiteLights(false);  delay(50);
     SetWhiteLights(true);
       RadMeter();
    for (int i = 0; i < 15; i++) {
          RadMeter();
          value_IR = analogRead(IR_Silica);
          if (value_IR < 300) {
            PossibilityOfSilicia = 1;
          }
            RadMeter();
    }
      Serial3.println(".3.");
   SetWhiteLights(true);  delay(50);  SetWhiteLights(false);  delay(50);   SetWhiteLights(true);  
   digitalWrite(ElectroMagnet, HIGH);
   for (int i = 0; i < 15; i++) {
         RadMeter();
         value_kufitarja = digitalRead(KufitarjaMagnet);
         if (value_kufitarja == 1) {
           PossibilityOfMagneticProperties = true;
         }
         delay(100);
   } 
     Serial3.println(".4.");delay(100);
     RadMeter();
   digitalWrite(ElectroMagnet, LOW);
      Serial3.println(".5.");
  ReportSampleScan();
SetWhiteLights(false); 
}

void ReportSampleScan() {
   String dataRaw = "&" + String(PossibilityOfSilicia) + ";" + String(PossibilityOfMagneticProperties) + ";";
   Serial.println(dataRaw);
   
     Serial3.println(dataRaw);
     
}


void SetUVLights(bool Status) {
    if (Status) {
      digitalWrite(UV_1, HIGH);
       digitalWrite(UV_2, HIGH);
          digitalWrite(UV_3, HIGH);
    }else {
   digitalWrite(UV_1, LOW);
       digitalWrite(UV_2, LOW);
          digitalWrite(UV_3, LOW);
    }
      RadMeter();
}

void SetWhiteLights(bool Status) {
     if (Status) {
      digitalWrite(white_light1, HIGH);
       digitalWrite(white_light2, HIGH);
     
    }else {
   digitalWrite(white_light1, LOW);
       digitalWrite(white_light2, LOW);
      
    }
      RadMeter();
}

void CommandTaken(String CommandData) {
  Serial.println(CommandData);
   if (CommandData == "49") {
       Serial3.flush();
       CreateSample();
   }

   if (CommandData.indexOf("2") > 0) {
            Serial3.flush();
     SampleAbortion = true;
   }
}
