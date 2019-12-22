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
            int tried = 0;
            string[] splitted = msg.Split(' ');
            if (splitted.Length == 6)
            {
                if (_connected && (int.TryParse(splitted[0], out tried) && int.TryParse(splitted[1], out tried) && int.TryParse(splitted[2], out tried) && int.TryParse(splitted[3], out tried) && int.TryParse(splitted[4], out tried) && int.TryParse(splitted[5], out tried))) //Checks if the arduino is connected and if every part of the message is an integer
                {
                    port.WriteLine(msg);
                }
            }
            return;
        }

        public string[] getAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
