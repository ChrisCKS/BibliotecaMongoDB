using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjetoBiblioteca.Livros
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Titulo { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]     //Guarda o ID do autor dentro do livro
        public string AutorId { get; set; }

        public int Ano { get; set; }
    }
}
