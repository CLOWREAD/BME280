using System;
using System.Collections.Generic;
using System.Text;

using System.Device.Gpio;


namespace Aqua_Lamp_Client
{
    class BME_280
    {
        int sda = 2;
        int scl = 3;
        byte bme280_addr;
        UInt16 dig_T1;
        Int16 dig_T2;
        Int16 dig_T3;

        UInt16 dig_P1;
        Int16 dig_P2;
        Int16 dig_P3;
        Int16 dig_P4;
        Int16 dig_P5;
        Int16 dig_P6;
        Int16 dig_P7;
        Int16 dig_P8;
        Int16 dig_P9;


        GpioController gpio;
        public void Init()
        {
            gpio = new GpioController();
            sda = Config_Client.m_Config.GPIO_BME280_SDA;
            scl = Config_Client.m_Config.GPIO_BME280_SCL;
            bme280_addr = (byte)Config_Client.m_Config.GPIO_BME280_ADDR;

            gpio.OpenPin(scl, PinMode.Output);
            gpio.OpenPin(sda, PinMode.Output);

            Send_To_Addr( 0xE0, 0xB6);

            Send_To_Addr( 0xf4, 0xff);
            Send_To_Addr( 0xf5, 0xe0);
            Send_To_Addr( 0xf2, 0x01);


             dig_T1 = Read_From_Addr( 0x89); dig_T1 <<= 8;
            dig_T1 += Read_From_Addr( 0x88);

             dig_T2 = Read_From_Addr( 0x8B); dig_T2 <<= 8;
            dig_T2 += Read_From_Addr( 0x8A);

             dig_T3 = Read_From_Addr( 0x8D); dig_T3 <<= 8;
            dig_T3 += Read_From_Addr( 0x8C);

             dig_P1 = Read_From_Addr( 0x8F); dig_P1 <<= 8;
            dig_P1 += Read_From_Addr( 0x8E);

             dig_P2 = Read_From_Addr( 0x91); dig_P2 <<= 8;
            dig_P2 += Read_From_Addr( 0x90);

             dig_P3 = Read_From_Addr( 0x93); dig_P3 <<= 8;
            dig_P3 += Read_From_Addr( 0x92);

             dig_P4 = Read_From_Addr( 0x95); dig_P4 <<= 8;
            dig_P4 += Read_From_Addr( 0x94);

             dig_P5 = Read_From_Addr( 0x97); dig_P5 <<= 8;
            dig_P5 += Read_From_Addr( 0x96);

             dig_P6 = Read_From_Addr( 0x99); dig_P6 <<= 8;
            dig_P6 += Read_From_Addr( 0x98);

             dig_P7 = Read_From_Addr( 0x9B); dig_P7 <<= 8;
            dig_P7 += Read_From_Addr( 0x9A);

             dig_P8 = Read_From_Addr( 0x9D); dig_P8 <<= 8;
            dig_P8 += Read_From_Addr( 0x9C);

             dig_P9 = Read_From_Addr( 0x9F); dig_P9 <<= 8;
            dig_P9 += Read_From_Addr( 0x9E);

        }
        Int32 m_Fine = 0;
        public Int32 GetTemperature()
        {
            UInt64 raw_temp = 0;
            raw_temp += Read_From_Addr(0xfA);
            raw_temp <<= 8;
            raw_temp += Read_From_Addr( 0xfB);
            raw_temp <<= 8;
            raw_temp += Read_From_Addr( 0xfC);
            raw_temp >>= 4;



            Int32 var1 = (Int32)((((Int32)raw_temp) >> 3) - (((UInt16)dig_T1) << 1));
            var1 *= ((Int16)dig_T2);
            var1 >>= 11;

            Int32 var2 = (Int32)((((Int32)raw_temp) >> 4) - ((UInt32)dig_T1));
            var2 *= var2;
            var2 >>= 12;
            var2 *= ((Int16)dig_T3);
            var2 >>= 14;
            var t_fine = var1 + var2;
            m_Fine = t_fine;
            var T = (t_fine * 5 + 128) >> 8;
            return T;
        }

        int I2C_IDLE = 1;
        public  void Start()
        {
            //gpio.Write(scl, PinValue.Low);
            //System.Threading.Thread.Sleep(I2C_IDLE);
            //gpio.Write(sda, PinValue.Low);
            //System.Threading.Thread.Sleep(I2C_IDLE);
            gpio.SetPinMode(sda, PinMode.Output);
            gpio.Write(scl, PinValue.High);
            gpio.Write(sda, PinValue.High);
            System.Threading.Thread.Sleep(I2C_IDLE);
            gpio.Write(sda, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);

            gpio.Write(scl, PinValue.Low);
            gpio.Write(sda, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);
        }
        public  void End()
        {
            gpio.SetPinMode(sda, PinMode.Output);
            gpio.Write(scl, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);
            gpio.Write(sda, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);

            gpio.Write(scl, PinValue.High);
            //gpio.Write(sda, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);
            gpio.Write(sda, PinValue.High);
            System.Threading.Thread.Sleep(I2C_IDLE);

            //gpio.Write(scl, PinValue.Low);
            //gpio.Write(sda, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);
        }
        public  byte Read_From_Addr( byte addr)
        {


            Start();
            Send( 0b11101100);

            Send( addr);

            Start();
            Send( 0b11101101);

            byte res = Read();

            End();
            return res;
        }
        public  void Send_To_Addr( byte addr, byte data)
        {


            Start();
            Send( 0b11101100);

            Send( addr);
            Send( data);

            End();

        }
        public  void Send( byte data)
        {

            gpio.SetPinMode(sda, PinMode.Output);
            int i = 0;
            byte temp = data;
            for (i = 0; i < 8; i++)
            {
                gpio.Write(scl, PinValue.Low);
                System.Threading.Thread.Sleep(I2C_IDLE);
                if ((temp & 0x80) == 0)
                {
                    //Console.WriteLine("0");

                    gpio.Write(sda, PinValue.Low);
                    System.Threading.Thread.Sleep(I2C_IDLE);

                }
                else
                {
                    //Console.WriteLine("1");

                    gpio.Write(sda, PinValue.High);
                    System.Threading.Thread.Sleep(I2C_IDLE);

                }
                gpio.Write(scl, PinValue.High);
                System.Threading.Thread.Sleep(I2C_IDLE);
                gpio.Write(scl, PinValue.Low);
                System.Threading.Thread.Sleep(I2C_IDLE);

                temp = (byte)(temp * 2);
            }
            gpio.Write(scl, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);
            gpio.Write(scl, PinValue.High);
            System.Threading.Thread.Sleep(I2C_IDLE);

            gpio.SetPinMode(sda, PinMode.Input);
            PinValue ack = gpio.Read(sda);
            //Console.WriteLine(ack.ToString());

            gpio.Write(scl, PinValue.Low);


        }
        public  byte Read()
        {


            gpio.SetPinMode(sda, PinMode.Input);
            int i = 0;
            byte temp = 0;
            PinValue ack;
            for (i = 0; i < 8; i++)
            {
                temp = (byte)(temp * 2);
                gpio.Write(scl, PinValue.Low);
                System.Threading.Thread.Sleep(I2C_IDLE);
                gpio.Write(scl, PinValue.High);
                System.Threading.Thread.Sleep(I2C_IDLE);
                PinValue data = gpio.Read(sda);
                temp += data.Equals(PinValue.High) ? (byte)0x01 : (byte)0x00;
                System.Threading.Thread.Sleep(I2C_IDLE);
                gpio.Write(scl, PinValue.Low);
                System.Threading.Thread.Sleep(I2C_IDLE);


            }
            gpio.Write(scl, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);
            gpio.Write(scl, PinValue.High);
            System.Threading.Thread.Sleep(I2C_IDLE);

            gpio.SetPinMode(sda, PinMode.Input);
            ack = gpio.Read(sda);
            //Console.WriteLine(ack.ToString());

            gpio.Write(scl, PinValue.Low);

            return temp;
        }
    }
}
