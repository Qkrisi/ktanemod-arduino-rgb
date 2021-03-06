﻿using System;
//using System.Timers;
using System.IO.Ports;

namespace rgbMod
{
    public class Arduino
    {
        public bool _connected = false;
        private SerialPort port;
        private bool ableToSend = true;
        /*private float time = 0f;
        private Timer readTimer;*/

        public void Connect(string name, int baudRate)
        {
            if (Array.IndexOf(getAvailablePorts(), name) > -1 && !_connected)
            {
                port = new SerialPort(name, baudRate);
                try
                {
                    port.Open();
                    _connected = true;
                }
                catch
                {
                    _connected = false;
                }
            }
            return;
        }

        public void Disconnect()
        {
            if (_connected)
            {
                try
                {
                    Stop();
                    port.Close();
                }
                finally
                {
                    _connected = false;
                }
            }
            return;
        }

        public void Stop()
        {
            if(_connected)
            {
                try
                {
                    sendMSG("9999999 9999999 9999999 0 0 0");
                }
                catch
                {
                    _connected = false;
                }
            }
        }

        /// <summary>
        /// The sendMSG method sends the colors that the arduino should display
        /// </summary>
        /// <param name="msg">Message. Syntax: "redPinNum greenPinNum bluePinNum redValueNum greenValueNum blueValueNum". 9999999 as red pin will turn the led off</param>
        public void sendMSG(string msg)
        {
            int tried = 0;
            string[] splitted = msg.Split(' ');
            if (splitted.Length == 6)
            {
                if (_connected && isAbleToSend() && (int.TryParse(splitted[0], out tried) && int.TryParse(splitted[1], out tried) && int.TryParse(splitted[2], out tried) && int.TryParse(splitted[3], out tried) && int.TryParse(splitted[4], out tried) && int.TryParse(splitted[5], out tried))) //Checks if the arduino is connected and if every part of the message is an integer
                {
                    try
                    {
                        port.WriteLine(msg+"\r\n");
                        ableToSend = false;
                    }
                    catch
                    {
                        ableToSend = true;
                        _connected = false;
                    }
                }
            }
            return;
        }

        /*public float Calibrate(string msg, bool up)
        {
            int tried = 0;
            time = up ? .1f : 0f;
            string[] splitted = msg.Split(' ');
            if (splitted.Length == 6)
            {
                if (_connected && isAbleToSend() && (int.TryParse(splitted[0], out tried) && int.TryParse(splitted[1], out tried) && int.TryParse(splitted[2], out tried) && int.TryParse(splitted[3], out tried) && int.TryParse(splitted[4], out tried) && int.TryParse(splitted[5], out tried))) //Checks if the arduino is connected and if every part of the message is an integer
                {
                    try
                    {
                        readTimer = new Timer(100);
                        readTimer.Elapsed += OnTimerEvent;
                        readTimer.AutoReset = true;
                        readTimer.Enabled = false;
                        port.WriteLine(msg+"\r\n");
                        ableToSend = false;
                        readTimer.Enabled = true;
                        port.ReadLine();
                        readTimer.Enabled = false;
                        ableToSend = true;
                        return time;

                    }
                    catch
                    {
                        ableToSend = true;
                        _connected = false;
                    }
                }
            }
            return 1.1f;
        }

        private void OnTimerEvent(System.Object source, ElapsedEventArgs e)
        {
            time += .1f;
        }*/

        public bool isAbleToSend()
        {
            if(!ableToSend && !(port.ReadLine() == ""))
            {
                ableToSend = true;
            }
            return ableToSend;
        }

        public string[] getAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
