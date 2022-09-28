using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Practica01.Models;
using Practica01.Models.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Dynamic;
using Classlibrary;
using Newtonsoft.Json;
using System.Collections;

namespace Practica01.Controllers
{

    public class PersonController : Controller
    {
        public delegate Person Edition(Person person1, Person person2);
        public delegate int dataEncode(Person person1, string company);

        Huffman huffmanT = new Huffman();
        public IActionResult Index()
        {
            return View(Singleton.Instance.AVLnames);
        }

        public IActionResult codificacion()
        {
            return View("codificacion");
        }
        public IActionResult Encodeview()
        {
            return View(Singleton.Instance.AVLnames);
        }

        public IActionResult Decodeview()
        {
            return View(Singleton.Instance.AVLnames);
        }

        //Funciones para cargar datos en archivos .csv

        public ActionResult Reading()
        {
            return View("LeerArchivo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LeerArchivo()
        {
            string path = @"C:\Users\nossu\Desktop\OR\inputfile.csv";
            //System.IO.StreamReader doc = new System.IO.StreamReader(path);
            string line = System.IO.File.ReadAllText(path);
            foreach(string row in line.Split('\n'))
            {
                if(!string.IsNullOrEmpty(row))
                {
                    string[] data = row.Split(';');
                    Person person = JsonConvert.DeserializeObject<Person>(data[1]);
                    if (data[0] == "INSERT")
                    {
                        Person newPerson = new Person();
                        newPerson.name = person.name;
                        newPerson.dpi = person.dpi;
                        newPerson.datebirth = person.datebirth;
                        newPerson.address = person.address;
                        newPerson.companies = person.companies;
                        Singleton.Instance.AVLnames.Insert(newPerson, newPerson.dpiComparer);
                    }
                    else if(data[0] == "PATCH")
                    {
                        
                        Edition patch = Person.PatchData;
                        Person newPerson = new Person();
                        newPerson.name = person.name;
                        newPerson.dpi = person.dpi;
                        newPerson.datebirth = person.datebirth;
                        newPerson.address = person.address;
                        Node<Person> newNode = new Node<Person>();
                        newNode.value = newPerson;
                        Singleton.Instance.AVLnames.Search(newNode, newPerson.dpiComparer);
                        Singleton.Instance.AVLnames.Patch(newPerson,newNode, newPerson.dpiComparer, patch);

                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search()
        {
            return View(Singleton.Instance.AVLnames);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string dpi)
        {
            try
            {
                if (dpi == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Person NewPerson = new Person();
                    NewPerson.name = dpi;
                    Node<Person> newNode = new Node<Person>();
                    newNode.value = NewPerson;
                    Singleton.Instance.AVLnames.Search(newNode, NewPerson.dpiComparer);
                    return RedirectToAction(nameof(Search));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Delete(string Persona)
        {
            Person nuevaPersona = new Person();
            nuevaPersona.dpi = Persona;
            Node<Person> nuevonodo = new Node<Person>();
            nuevonodo.value = nuevaPersona;
            Singleton.Instance.AVLnames.Delete(nuevaPersona, nuevaPersona.dpiComparer);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncontrarDPICodificado(string encodeData)
        {
            Singleton.Instance.AVLnames.Encoded.Clear();
            try
            {
                dataEncode validate = Person.dataEncode;
                if (encodeData == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Singleton.Instance.AVLnames.InOrder2(validate, encodeData);
                    for(int i = 0; i < Singleton.Instance.AVLnames.CompaniesTree.Count; i++)
                    {
                        string final = "";
                        string concatenado = encodeData+""+Singleton.Instance.AVLnames.CompaniesTree[i].dpi;
                        huffmanT.Build(concatenado);
                        BitArray encoded = huffmanT.Encode(concatenado);
                        foreach(bool bit in encoded)
                        {
                            final = final+""+((bit ? 1 : 0) + "");
                        }
                        Singleton.Instance.AVLnames.Encoded.Add(final);
                        //Singleton.Instance.AVLnames.CompaniesTree[i].encode[i] = final;
                    }
                    return RedirectToAction(nameof(Encodeview));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncontrarDPIDecodificado(string decodeData)
        {
            Singleton.Instance.AVLnames.Encoded.Clear();
            Singleton.Instance.AVLnames.Decoded.Clear();
            try
            {
                dataEncode validate = Person.dataEncode;
                if (decodeData == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Singleton.Instance.AVLnames.InOrder2(validate, decodeData);
                    for (int i = 0; i < Singleton.Instance.AVLnames.CompaniesTree.Count; i++)
                    {
                        string final = "";
                        string concatenado = decodeData + "" + Singleton.Instance.AVLnames.CompaniesTree[i].dpi;
                        huffmanT.Build(concatenado);
                        BitArray encoded = huffmanT.Encode(concatenado);
                        foreach (bool bit in encoded)
                        {
                            final = final + "" + ((bit ? 1 : 0) + "");
                        }
                        Singleton.Instance.AVLnames.Encoded.Add(final);
                        string decoded = huffmanT.Decode(encoded);
                        Singleton.Instance.AVLnames.Decoded.Add(decoded);
                    }
                    return RedirectToAction(nameof(Decodeview));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

    }
}