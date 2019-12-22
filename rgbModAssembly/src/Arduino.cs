using System;
using System.IO.Ports;

namespace rgbMod
{
    public class Arduino
    {
        public bool _connected = false;
        private SerialPort port;

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
                port.Close();
                _connected = false;
            }
            return;
        }

        /// <summary>
        /// The sendMSG method sends the colors that the arduino should display
        /// </summary>
        /// <param name="msg">Message. Syntax: "redPinNum greenPinNum bluePinNum redValueNum greenValueNum blueValueNum"</param>


        public void sendMSG(string msg)
        {
            if (_connected)
            {
                port.WriteLine(msg);
            }
            return;
        }

        public string[] getAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
