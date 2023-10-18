#include <Adafruit_GFX.h>
#include <Adafruit_NeoMatrix.h>
#include <Adafruit_NeoPixel.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>

const char* ssid = "";
const char* password = "";
const char* mqtt_server = "dev.rightech.io";
const char* mqtt_topic_ascii = "base/state/ascii";
const char* mqtt_topic_colour = "base/state/colour";
const char* mqtt_topic_brightness = "base/state/brightness";
const char* mqtt_client_id = "";

WiFiClient espClient;
PubSubClient client(espClient);

#define PIN            2
#define NUM_LEDS      256 

Adafruit_NeoMatrix matrix = Adafruit_NeoMatrix(16, 16, PIN,
  NEO_MATRIX_TOP     + NEO_MATRIX_LEFT +
  NEO_MATRIX_COLUMNS + NEO_MATRIX_ZIGZAG,
  NEO_GRB + NEO_KHZ800);

uint32_t currentColor = matrix.Color(255, 255, 255);
char currentLetter = ' ';

void setup() 
{
  Serial.begin(9600);

  matrix.begin();
  matrix.fillScreen(0);
  matrix.show();

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }
  Serial.println("Connected to WiFi");

  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);

  while (!client.connected()) 
  {
    if (client.connect(mqtt_client_id))
    {
      Serial.println("Connected to MQTT broker");
      client.subscribe(mqtt_topic_ascii);
      client.subscribe(mqtt_topic_colour);
      client.subscribe(mqtt_topic_brightness);
    }
    else
    {
      Serial.println("Failed to connect to MQTT broker, retrying in 5 seconds...");
      delay(5000);
    }
  }
}

void loop()
{
  client.loop();
}

void callback(char* topic, byte* payload, unsigned int length) 
{
  Serial.print("Message arrived in topic: ");
  Serial.println(topic);

  String receivedMessage = "";
  for (unsigned int i = 0; i < length; i++) 
  {
    receivedMessage += (char)payload[i];
  }

  Serial.print("Received message: ");
  Serial.println(receivedMessage);

  if (strcmp(topic, mqtt_topic_ascii) == 0) 
  {
    if (receivedMessage.equalsIgnoreCase("OFF")) 
    {
      currentLetter = ' ';
    }
    else 
    {
      currentLetter = receivedMessage[0];
    }
    displayLetterWithZigzag(currentLetter, currentColor, 2);
    matrix.show();
  }
  else if (strcmp(topic, mqtt_topic_colour) == 0) 
  {
    if (receivedMessage.equalsIgnoreCase("red")) 
    {
      currentColor = matrix.Color(255, 0, 0);
    }
    else if (receivedMessage.equalsIgnoreCase("green")) 
    {
      currentColor = matrix.Color(0, 255, 0);
    }
    else if (receivedMessage.equalsIgnoreCase("blue")) 
    {
      currentColor = matrix.Color(0, 0, 255);
    }
    else if (receivedMessage.equalsIgnoreCase("white")) 
    {
      currentColor = matrix.Color(255, 255, 255);
    }

    displayLetterWithZigzag(currentLetter,currentColor, 2);
  }
  else if (strcmp(topic, mqtt_topic_brightness) == 0) 
  {
    int brightness = 0;
    if (receivedMessage.equalsIgnoreCase("25%")) 
    {
      brightness = 8;
    }
    else if (receivedMessage.equalsIgnoreCase("50%")) 
    {
      brightness = 64;
    }
    else if (receivedMessage.equalsIgnoreCase("75%")) 
    {
      brightness = 128;
    }
    else if (receivedMessage.equalsIgnoreCase("100%")) 
    {
      brightness = 255;
    }

    matrix.setBrightness(brightness);
  }
  
  matrix.show();
}

void displayLetterWithZigzag(char letter, uint32_t color, uint8_t scale) 
{
  matrix.setTextSize(scale);
  matrix.fillScreen(0);
  matrix.setTextColor(color);
  matrix.setCursor(3, 1);
  
  matrix.print(letter);
  
  matrix.show();
}
