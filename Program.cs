using System;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Device.Gpio;
using System.Threading;

namespace Aqua_Lamp_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Config_Client.Load();
            BME280Loop bme_loop = new BME280Loop();
           
            
            QueryLoop ql = new QueryLoop();

            Thread bme_thread = new Thread(() =>
              {
                   bme_loop.Start().Wait();
              });
            bme_thread.Start();
            Thread query_thread = new Thread(() =>
            {
                ql.Start().Wait();
            });
            query_thread.Start();
            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
    class QueryLoop
    {
        bool Loop_Enable = true;
        public async Task Start()
        {
            Lit t_Lit = new Lit();
            GpioController controller = new GpioController();

            controller.OpenPin(Config_Client.m_Config.GPIO_LIT, PinMode.Output);
            Aqua_Lamp_Lit_Json allj_global = new Aqua_Lamp_Lit_Json();
            Loop_Enable = true;
            while (Loop_Enable)
            {
                try
                {

                var allj = await t_Lit.Get_Aqua_Lamp_Lit(Config_Client.m_Config.client_id);
                allj_global = await t_Lit.Get_Aqua_Lamp_Lit("GLOBAL");
                if (allj.Lit.Equals("On"))
                {
                    controller.Write(Config_Client.m_Config.GPIO_LIT, PinValue.Low);
                }
                if (allj.Lit.Equals("Off"))
                {
                    controller.Write(Config_Client.m_Config.GPIO_LIT, PinValue.High);
                }
                if (allj.Lit.Equals("High Impedance"))
                {
                    bool lamp_lit = false;
                    String[] lit_array = allj_global.Script.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    for(int lit_array_index=0; lit_array_index<lit_array.Length; lit_array_index++ )
                    {
                        String[] time_zone = lit_array[lit_array_index].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        Int64 tz_from = Convert.ToInt64(time_zone[0]);
                        Int64 tz_to = Convert.ToInt64(time_zone[1]);
                        var t_now = DateTime.Now;
                        Int64 t_second = t_now.Hour * 3600 + t_now.Minute * 60 + t_now.Second;
                        if(t_second>=tz_from && t_second<=tz_to)
                        {
                            lamp_lit = true;
                        }

                    }

                    if(lamp_lit)
                    {
                        controller.Write(Config_Client.m_Config.GPIO_LIT, PinValue.Low);
                    }else
                    {
                        controller.Write(Config_Client.m_Config.GPIO_LIT, PinValue.High);
                    }

                }
                Thread.Sleep(3000);
                }catch(Exception e)
                {
                    bool lamp_lit = false;
                    String[] lit_array = allj_global.Script.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    for (int lit_array_index = 0; lit_array_index < lit_array.Length; lit_array_index++)
                    {
                        String[] time_zone = lit_array[lit_array_index].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        Int64 tz_from = Convert.ToInt64(time_zone[0]);
                        Int64 tz_to = Convert.ToInt64(time_zone[1]);
                        var t_now = DateTime.Now;
                        Int64 t_second = t_now.Hour * 3600 + t_now.Minute * 60 + t_now.Second;
                        if (t_second >= tz_from && t_second <= tz_to)
                        {
                            lamp_lit = true;
                        }

                    }

                    if (lamp_lit)
                    {
                        controller.Write(Config_Client.m_Config.GPIO_LIT, PinValue.Low);
                    }
                    else
                    {
                        controller.Write(Config_Client.m_Config.GPIO_LIT, PinValue.High);
                    }
                    Console.WriteLine("L");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Data);
                }
            }

        }
        public void Stop()
        {
            Loop_Enable = false;
        }

    }

    class BME280Loop
    {
        bool Loop_Enable = true;
        BME_280 m_BME = new BME_280();
        public async Task Start()
        {
            
            Loop_Enable = true;
            m_BME.Init();
            while (Loop_Enable)
            {
                try
                {
                   float t_temp = m_BME.GetTemperature();
                    await Set_Temperature(t_temp);
                    Thread.Sleep(3000);
                }
                catch (Exception e)
                {
                    Console.WriteLine("T");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Data);
                }
            }

        }
        public void Stop()
        {
            Loop_Enable = false;
        }
        public async Task Set_Temperature(float temperature)
        {
            Console.WriteLine("{0}", temperature);
            HttpClient hc = new HttpClient();
            Aqua_Lamp_Lit_Json allj = new Aqua_Lamp_Lit_Json();
            allj.Lamp_ID = Config_Client.m_Config.client_id;
            allj.Temperature = temperature.ToString();
            HttpContent http_content = new StringContent(JsonHelper.ToJson(allj,typeof(Aqua_Lamp_Lit_Json)) );
            var res = await hc.PostAsync(Config_Client.m_Config.server_endpoint + "/api/lit/set_by_id/" + Config_Client.m_Config.client_id, http_content);
            
        }
    }

    class LED_Blob
    {
        public static void Lit()
        {

            
        }
    }
    class LED_Matrix
    {
        static int I2C_IDLE = 1;
        public static void Heart()
        {
            int scl = 2;
            int sda = 3;


            GpioController controller = new GpioController();

            controller.OpenPin(scl, PinMode.Output);
            controller.OpenPin(sda, PinMode.Output);
            //controller.Read(scl);
            //controller.OpenPin(sda, PinMode.Input);


            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b01000000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b10001000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b11000000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b01000000);
            Send(controller, scl, sda, 0b00100100);
            Send(controller, scl, sda, 0b01111110);
            Send(controller, scl, sda, 0b11111111);
            Send(controller, scl, sda, 0b11111111);
            Send(controller, scl, sda, 0b01111110);
            Send(controller, scl, sda, 0b00111100);
            Send(controller, scl, sda, 0b00011000);
            Send(controller, scl, sda, 0b00000000);

            End(controller, scl, sda);
            Console.Read();
        }
        public static void Circle()
        {
            int scl = 2;
            int sda = 3;


            GpioController controller = new GpioController();

            controller.OpenPin(scl, PinMode.Output);
            controller.OpenPin(sda, PinMode.Output);
            //controller.Read(scl);
            //controller.OpenPin(sda, PinMode.Input);


            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b01000000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b10001000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b11000000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b01000000);
            Send(controller, scl, sda, 0b00111100);
            Send(controller, scl, sda, 0b01111110);
            Send(controller, scl, sda, 0b11111111);
            Send(controller, scl, sda, 0b11111111);
            Send(controller, scl, sda, 0b01111110);
            Send(controller, scl, sda, 0b00111100);
            Send(controller, scl, sda, 0b00000000);
            Send(controller, scl, sda, 0b00000000);

            End(controller, scl, sda);
            Console.Read();
        }

        public static void Empty()
        {
            int scl = 2;
            int sda = 3;


            GpioController controller = new GpioController();

            controller.OpenPin(scl, PinMode.Output);
            controller.OpenPin(sda, PinMode.Output);
            //controller.Read(scl);
            //controller.OpenPin(sda, PinMode.Input);


            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b01000000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b10001000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b11000000);
            End(controller, scl, sda);
            ////
            Start(controller, scl, sda);
            Send(controller, scl, sda, 0b01000000);
            Send(controller, scl, sda, 0b00000000);
            Send(controller, scl, sda, 0b00000000);
            Send(controller, scl, sda, 0b00000000);
            Send(controller, scl, sda, 0b00000000);
            Send(controller, scl, sda, 0b00000000);
            Send(controller, scl, sda, 0b00000000);
            Send(controller, scl, sda, 0b00000000);
            Send(controller, scl, sda, 0b00000000);

            End(controller, scl, sda);
            Console.Read();
        }
        static void Start(GpioController gpio, int scl, int sda)
        {
            //gpio.Write(scl, PinValue.Low);
            //System.Threading.Thread.Sleep(I2C_IDLE);
            //gpio.Write(sda, PinValue.Low);
            //System.Threading.Thread.Sleep(I2C_IDLE);

            gpio.Write(scl, PinValue.High);
            gpio.Write(sda, PinValue.High);
            System.Threading.Thread.Sleep(I2C_IDLE);
            gpio.Write(sda, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);

            gpio.Write(scl, PinValue.Low);
            gpio.Write(sda, PinValue.Low);
            System.Threading.Thread.Sleep(I2C_IDLE);
        }
        static void End(GpioController gpio, int scl, int sda)
        {
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
        static void Send(GpioController gpio, int scl, int sda, byte data)
        {


            int i = 0;
            byte temp = data;
            for (i = 0; i < 8; i++)
            {
                if ((temp & 0x01) == 0)
                {
                    gpio.Write(scl, PinValue.Low);
                    System.Threading.Thread.Sleep(I2C_IDLE);
                    gpio.Write(sda, PinValue.Low);
                    System.Threading.Thread.Sleep(I2C_IDLE);
                    gpio.Write(scl, PinValue.High);
                    System.Threading.Thread.Sleep(I2C_IDLE);
                    gpio.Write(scl, PinValue.Low);
                    System.Threading.Thread.Sleep(I2C_IDLE);
                }
                else
                {
                    gpio.Write(scl, PinValue.Low);
                    System.Threading.Thread.Sleep(I2C_IDLE);
                    gpio.Write(sda, PinValue.High);
                    System.Threading.Thread.Sleep(I2C_IDLE);
                    gpio.Write(scl, PinValue.High);
                    System.Threading.Thread.Sleep(I2C_IDLE);
                    gpio.Write(scl, PinValue.Low);
                    System.Threading.Thread.Sleep(I2C_IDLE);
                }
                temp = (byte)(temp / 2);
            }

        }
    }

}
