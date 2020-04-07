using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ConsumirAPIRefit.Classes.Login;
using ConsumirAPIRefit.Classes.Users;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ConsumirAPIRefit
{
    class Program
    {
        private static string _urlBase;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile($"appsettings.json");
            var config = builder.Build();
            _urlBase = config.GetSection("ApiAcess:UrlBase").Value;

            try
            {
                string email = string.Empty;

                // Console.Clear();
                Console.WriteLine("Consumindo API REST - Exemplo");
                email = SolicitarEmail();

                Console.WriteLine("Dados: uri - email");
                Console.WriteLine($"{_urlBase} -  {email} \n");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();

                    //incluir o cabeçalho Accept que sera enviado na requisição             
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Envio da requisição para receber token
                    HttpResponseMessage respToken = client.PostAsync(_urlBase + "Login", new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        email = email,
                    }), Encoding.UTF8, "application/json")).Result;

                    //obtem o token gerado
                    string conteudo = respToken.Content.ReadAsStringAsync().Result;

                    Console.WriteLine(conteudo + "\n");

                    if (respToken.StatusCode == HttpStatusCode.OK)
                    {
                        //deserializa o token
                        Token token = JsonConvert.DeserializeObject<Token>(conteudo);

                        bool autorizado = false;
                        Boolean.TryParse(token.Authenticated, out autorizado);

                        if (autorizado)
                        {
                            Console.Clear();
                            
                            Console.WriteLine("===========");
                            Console.WriteLine("Autenticado");
                            Console.WriteLine("===========");

                            // Associar o token aos headers do objeto
                            // do tipo HttpClient
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AcessToken);

                            bool ret = true;
                            while (ret)
                            {
                                int aux = int.MinValue;
                                Console.Write(
    @"
Testar metodo
1-GetAll
2-GetId
3-Post
4-Put
5-Delete

0-Sair
");

                                if (int.TryParse(Console.ReadLine(), out aux) && 0 < aux && aux < 6)
                                {
                                    switch (aux)
                                    {
                                        case 1:
                                            GetAll(client);
                                            break;
                                        case 2:
                                            GetById(client);
                                            break;
                                        case 3:
                                            Post(client);
                                            break;
                                        case 4:
                                            Put(client);
                                            break;
                                        case 5:
                                            Delete(client);
                                            break;
                                    }
                                }
                                else if (aux == 0)
                                {
                                    ret = false;
                                }
                                else
                                {
                                    Console.WriteLine("Opcao invalida");
                                }

                            }
                        }
                        else
                        {
                            Console.WriteLine("Nao autenticado");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Não foi possivel autenticar o usuario");
                    }

                }
                Console.WriteLine("Fim");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine("O programa acionou uma exceção");
                Console.Write(ex);
                Console.ReadLine();
            }

        }

        private static void GetAll(HttpClient client)
        {
            Console.WriteLine();
            HttpResponseMessage resp = client.GetAsync(_urlBase + "Users").Result;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                List<User> listUsers = JsonConvert.DeserializeObject<List<User>>(resp.Content.ReadAsStringAsync().Result);
                if (listUsers.Count == 0)
                {
                    Console.WriteLine("Nao possui registro");
                }
                else
                {
                    int aux = 1;
                    foreach(var item in listUsers)
                    {
                        Console.WriteLine("{0})", aux);
                        Console.WriteLine(item.ToString());
                        aux++;
                    }
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Nao foi possivel carregar os usuarios");
            }
        }

        private static void GetById(HttpClient client)
        {
            Console.WriteLine();
            Console.WriteLine("Informe um Id");
            string Id = string.Empty;
            do
            {
                Id = Console.ReadLine();
            } while (Id == string.Empty);
            HttpResponseMessage resp = client.GetAsync(_urlBase + "Users" + "/" + Id).Result;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                User user;
                try
                {
                    user = JsonConvert.DeserializeObject<User>(resp.Content.ReadAsStringAsync().Result);   
                }
                catch
                {
                    Console.WriteLine("");
                    Console.WriteLine("Nao foi possivel encontrar o usuario");
                    return;
                }

                if (user == null)
                {
                    Console.WriteLine("Registro nao encontrado");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(user.ToString());
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Nao foi possivel encontrar o usuario");
            }
        }

        private static void Post(HttpClient client)
        {
            Console.WriteLine();
            Console.WriteLine("Informe o nome:");
            string name = Console.ReadLine();
            Console.WriteLine("Informe um email:");
            string email = Console.ReadLine();

            HttpResponseMessage resp = 
                client.PostAsync
                    (
                        _urlBase + "Users",
                        new StringContent
                        (
                            JsonConvert.SerializeObject
                            (
                                new 
                                {
                                    name = name,
                                    email = email
                                }
                            ), Encoding.UTF8, "application/json"
                        )
                    )
                    .Result;

            if (resp.StatusCode == HttpStatusCode.Created)
            {
                User user = JsonConvert.DeserializeObject<User>(resp.Content.ReadAsStringAsync().Result);
                Console.WriteLine("Usuario criado");
                Console.WriteLine();
                Console.WriteLine(user.ToString());
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Nao foi possivel criar o usuario");
            }
        }

        private static void Put(HttpClient client)
        {
            string id = string.Empty;
            string name = string.Empty;
            string email = string.Empty;
            Console.WriteLine();
            Console.WriteLine("Informe o Id:");
            id = Console.ReadLine();
            Console.WriteLine("Informe o nome para atualizacao:");
            name = Console.ReadLine();
            Console.WriteLine("Informe o email para atualizacao:");
            email = Console.ReadLine();
            
            HttpResponseMessage resp = 
                client.PutAsync
                    (
                        _urlBase + "Users",
                        new StringContent
                        (
                            JsonConvert.SerializeObject
                            (
                                new 
                                {
                                    id = id,
                                    name = name,
                                    email = email
                                }
                            ), Encoding.UTF8, "application/json"
                        )
                    )
                    .Result;


            if (resp.StatusCode == HttpStatusCode.OK)
            {
                User user = JsonConvert.DeserializeObject<User>(resp.Content.ReadAsStringAsync().Result);
                Console.WriteLine("Usuario atualizado");
                Console.WriteLine();
                Console.WriteLine(user.ToString());
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Nao foi possivel atualizar o usuario");
            }
        }

        private static void Delete(HttpClient client)
        {
            Console.WriteLine();
            Console.WriteLine("Informe um Id para deletar:");
            string id = Console.ReadLine();

            HttpResponseMessage resp = client.DeleteAsync(_urlBase + "Users" + "/" + id).Result;
            
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Usuario deletado!");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Nao foi possivel deltear o usuario");
            }
        }

        private static string SolicitarEmail()
        {
            string email;
            Console.WriteLine("Informe o email: ");
            email = Console.ReadLine();
            return email;
        }
    }
}
