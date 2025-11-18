using MongoDB.Bson;
using MongoDB.Driver;
using ProjetoBiblioteca.Autores;
using ProjetoBiblioteca.Livros;

class Program
{
    //Coleções no Mongo(tabelas)    //permite manipular os dados dentro das coleções do MongoDB
    static IMongoCollection<Author> TabelaAutores;
    static IMongoCollection<Book> TabelaLivros;

    //Dentro do metodo(async) uso await para chamar metodos assincronos
    //Task retorna "void" nos metodos assincronos
    static async Task Main(string[] args)
    {
        //Conexão com o Mongo
        var conexao = new MongoClient("mongodb://localhost:27017/");

        //Acessa o banco de dados
        var database = conexao.GetDatabase("ProjetoBiblioteca");

        //Acessa as coleções (tabelas)
        TabelaAutores = database.GetCollection<Author>("Autores");
        TabelaLivros = database.GetCollection<Book>("Livros");


        int opcao = -1;

        while (opcao != 0)
        {
            Console.WriteLine("MENU");
            Console.WriteLine("1 - Inserir Autor");
            Console.WriteLine("2 - Inserir Livro");
            Console.WriteLine("3 - Listar Autores");
            Console.WriteLine("4 - Listar Livros com Autor");
            Console.WriteLine("5 - Atualizar Autor");
            Console.WriteLine("6 - Deletar Autor");
            Console.WriteLine("7 - Deletar Livro");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out opcao))
            {
                Console.WriteLine("Opção inválida!\n");
                continue;
            }

            Console.WriteLine();

            switch (opcao)
            {
                case 1:
                    await InserirAutor();
                    break;
                case 2: 
                    await InserirLivro();
                    break;
                case 3:
                    await ListarAutores();
                    break;
                case 4:
                    await ListarLivros();
                    break;
                case 5:
                    await AtualizarAutor();
                    break;
                case 6:
                    await DeletarAutor();
                    break;
                case 7:
                    await DeletarLivro();
                    break;
                case 0:
                    Console.WriteLine("Saindo");
                    break;
                default:
                    Console.WriteLine("Opção inválida\n");
                    break;
            }

            Console.WriteLine();
        }
    }

    static async Task InserirAutor()
    {
        Console.Write("Nome do autor: ");
        string nome = Console.ReadLine();

        Console.Write("País do autor: ");
        string pais = Console.ReadLine();

        var autor = new Author { Nome = nome, Pais = pais };    //criando o objeto autor

        await TabelaAutores.InsertOneAsync(autor);  //inserindo o autor na tabela no MongoDB

        Console.WriteLine($"Autor '{autor.Nome}' inserido");
    }

    static async Task InserirLivro()
    {
        Console.Write("Título do livro: ");
        string titulo = Console.ReadLine();

        Console.Write("Ano de publicação: ");
        int ano = int.Parse(Console.ReadLine());

        Console.Write("ID do Autor: ");
        string autorId = Console.ReadLine();

        // valida se autor existe                //procura o primeiro autor que tenha o Id igual ao autorId
        var autor = await TabelaAutores.Find(a => a.Id == autorId).FirstOrDefaultAsync();
                                                                   
        if (autor == null)                                          
        {
            Console.WriteLine("Autor não encontrado.");
            return;
        }

        var livro = new Book {Titulo = titulo, Ano = ano, AutorId = autorId };

        await TabelaLivros.InsertOneAsync(livro);

        Console.WriteLine($"Livro '{titulo}' inserido");
    }


    static async Task ListarAutores()
    {                                     //buscando TODOS os autores na tabela Autores
        var autores = await TabelaAutores.Find(_ => true).ToListAsync();
                                                            //Transforma o resultado do MongoDB em uma lista            
        Console.WriteLine("AUTORES: ");
        foreach (var a in autores)     
        {
            Console.WriteLine($"ID: {a.Id} - Nome: {a.Nome} - País: {a.Pais}");
        }
    }

    static async Task ListarLivros()
    {
        var livros = await TabelaLivros.Find(_ => true).ToListAsync();
        var autores = await TabelaAutores.Find(_ => true).ToListAsync();

        Console.WriteLine("LIVROS: ");
        foreach (var l in livros)
        {
            var autor = autores.FirstOrDefault(a => a.Id == l.AutorId);

            string nomeAutor;

            if (autor != null)
            {
                nomeAutor = autor.Nome;
            }
            else
            {
                nomeAutor = "Autor não encontrado";
            }

            Console.WriteLine($"ID: {l.Id} - Título: {l.Titulo} - Ano: {l.Ano} - Autor: {nomeAutor}");
        }
    }

    static async Task AtualizarAutor()
    {
        Console.Write("ID do autor: ");
        string id = Console.ReadLine();

        Console.Write("Novo país: ");
        string novoPais = Console.ReadLine();

        //Criado um filtro onde o id do autor seja igual ao valor id que foi digitado"
        var filtro = Builders<Author>.Filter.Eq(a => a.Id, id);

        //Atualizando o Pais do autor com o valor novoPais
        var update = Builders<Author>.Update.Set(a => a.Pais, novoPais);

        //Buscando um autor que "bate" com o filtro, e aplicando a atualização"
        var atualizado = await TabelaAutores.UpdateOneAsync(filtro, update);

        //n° de documentos modificados
        if (atualizado.ModifiedCount > 0)
        {
            Console.WriteLine("Autor atualizado com sucesso!");
        }
        else
        {
            Console.WriteLine("Autor não encontrado!");
        }

    }


    static async Task DeletarAutor()
    {
        Console.Write("ID do autor: ");
        string id = Console.ReadLine();

        var deletado = await TabelaAutores.DeleteOneAsync(a => a.Id == id);

        //n° de documentos deletados
        if (deletado.DeletedCount > 0)
        {
            Console.WriteLine("Autor removido");
        }
        else
        {
            Console.WriteLine("Autor não encontrado");
        }
    }


    static async Task DeletarLivro()
    {
        Console.Write("ID do livro: ");
        string id = Console.ReadLine();

        var deletado = await TabelaLivros.DeleteOneAsync(b => b.Id == id);

        if (deletado.DeletedCount > 0)
        {
            Console.WriteLine("Livro Removido");
        }
        else
        {
            Console.WriteLine("Livro não encontrado");
        }
    }
}