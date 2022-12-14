using Classlibrary;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Practica01.Models;
using Practica01.Models.Data;
using System.Collections.Generic;
using System.IO;

namespace Practica01.Controllers
{

    public class PersonController : Controller
    {

        public delegate Person Edition(Person person1, Person person2);
        public delegate int dataEncode(Person person1, string company);
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
            string path = @"C:\Users\nossu\Desktop\input2.csv";
            //System.IO.StreamReader doc = new System.IO.StreamReader(path);
            string line = System.IO.File.ReadAllText(path);
            foreach (string row in line.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
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
                    else if (data[0] == "PATCH")
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
                        Singleton.Instance.AVLnames.Patch(newPerson, newNode, newPerson.dpiComparer, patch);

                    }
                }
            }
            return RedirectToAction(nameof(Index));
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

        //Método para la vista de  la búsqueda por dpi
        public IActionResult Search()
        {
            return View(Singleton.Instance.AVLnames);
        }

        //Método set para la búsqueda del dpi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string Recluta)
        {
            Singleton.Instance.AVLnames.CartList.Clear();
            try
            {
                if (Recluta == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Person NewPerson = new Person();
                    NewPerson.dpi = Recluta;
                    Node<Person> newNode = new Node<Person>();
                    newNode.value = NewPerson;
                    Singleton.Instance.AVLnames.Search(newNode, NewPerson.dpiComparer);

                    DirectoryInfo di = new DirectoryInfo(@"C:\Users\nossu\Desktop\inputs");
                    FileInfo[] files = di.GetFiles("*.txt");
                    foreach (FileInfo file in files)
                    {
                        if (file.Name.Contains(Recluta))
                        {
                            Singleton.Instance.AVLnames.CartList.Add(file.Name);
                        }
                    }
                    return RedirectToAction(nameof(Search));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Encode(string Recruit)
        {
            string path = @"C:\Users\nossu\Desktop\inputs\" + Recruit;
            string encodedfile = @"C:\Users\nossu\Desktop\compressed\compressed-" + Recruit;
            string decodedfile = @"C:\Users\nossu\Desktop\decoded\decoded-" + Recruit;
            try
            {
                if (Recruit == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Encoder encoder = new Encoder();
                    Decoder decoder = new Decoder();
                    string text = System.IO.File.ReadAllText(path, System.Text.ASCIIEncoding.Default);
                    List<int> compressed = encoder.Compress(text);
                    string compressedMessage = string.Join("", compressed);
                    System.IO.File.WriteAllText(encodedfile, compressedMessage);
                    return RedirectToAction(nameof(Search));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Decode(string Recruit)
        {
            string path = @"C:\Users\nossu\Desktop\inputs\" + Recruit;
            string encodedfile = @"C:\Users\nossu\Desktop\compressed\compressed-"+Recruit;
            string decodedfile = @"C:\Users\nossu\Desktop\decoded\decoded-"+Recruit;
            try
            {
                if (Recruit == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Encoder encoder = new Encoder();
                    Decoder decoder = new Decoder();
                    string text = System.IO.File.ReadAllText(path, System.Text.ASCIIEncoding.Default);
                    List<int> compressed = encoder.Compress(text);
                    string compressedMessage = string.Join("", compressed);
                    string decompressed = decoder.Decompress(compressed);
                    System.IO.File.WriteAllText(decodedfile, decompressed);
                    return RedirectToAction(nameof(Search));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

    }
}