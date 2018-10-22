﻿using ShareBook.Domain;
using ShareBook.Domain.Enums;
using ShareBook.Service;
using System;
using System.Text;
using Xunit;

namespace ShareBook.Test.Unit.Services
{
    public class EmailTemplateTests
    {
        readonly IEmailTemplate emailTemplate;

        private User user;
        private Book book;
        private User administrator;
        private User requestingUser;
        private ContactUs contactUs;

        public EmailTemplateTests()
        {
            emailTemplate = new EmailTemplate();

            user = new User()
            {
                Id = new Guid("5489A967-9320-4350-E6FC-08D5CC8498F3"),
                Name = "Rodrigo",
                Email = "rodrigo@sharebook.com",
                Linkedin = "linkedin.com/rodrigo",
                Profile = Profile.User
            };

            book = new Book()
            {
                Title = "Lord of the Rings",
                Author = "J. R. R. Tolkien",
                ImageSlug = "lotr.png",
                ImageBytes = Encoding.UTF8.GetBytes("STRINGBASE64"),
                User = user
            };

            requestingUser = new User()
            {

                Id = new Guid("5489A967-9320-4350-FFFF-08D5CC8498F3"),
                Name = "Walter Vinicius",
                Email = "walter@sharebook.com",
                Linkedin = "linkedin.com/walter",
                Profile = Profile.User
            };

            administrator = new User()
            {
                Id = new Guid("5489A967-AAAA-BBBB-CCCC-08D5CC8498F3"),
                Name = "Cussa Mitre",
                Email = "cussa@sharebook.com",
                Profile = Profile.Administrator
            };

            contactUs = new ContactUs()
            {
                Name = "Rafael Rocha",
                Email = "rafael@sharebook.com.br",
                Message = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident",
                Phone = "(11) 954422-2765"
            };

        }

        [Fact]
        public void VerifyEmailNewBookInsertedParse()
        {
            var result = emailTemplate.GenerateHtmlFromTemplateAsync("NewBookInsertedTemplate", book).Result;
            //<!DOCTYPE html>\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>Novo livro cadastrado - Sharebook</title>\r\n</head>\r\n<body>\r\n    <p>\r\n        Olá Cussa Mitre,\r\n    </p>\r\n    <p>\r\n        Um novo livro foi cadastrado. Veja mais informações abaixo:\r\n    </p>\r\n\r\n    <ul>\r\n        <li><strong>Livro: </strong>Lord of the Rings</li>\r\n        <li><strong>Autor: </strong>J. R. R. Tolkien</li>\r\n        <li><strong>Usuário: </strong>Rodrigo</li>\r\n    </ul>\r\n\r\n    <p>Sharebook</p>\r\n</body>\r\n</html>

            Assert.Contains("Olá Administrador(a),", result);
            Assert.Contains("<li><strong>Livro: </strong>Lord of the Rings</li>", result);
            Assert.Contains("<li><strong>Autor: </strong>J. R. R. Tolkien</li>", result);
            Assert.Contains("<li><strong>Usuário: </strong>Rodrigo</li>", result);
        }

        [Fact]
        public void VerifyEmailBookRequestedParse()
        {
            var vm = new
            {
                Book = book,
                RequestingUser = requestingUser,
                Administrator = administrator
            };

            var result = emailTemplate.GenerateHtmlFromTemplateAsync("BookRequestedTemplate", vm).Result;
            //<!DOCTYPE html>\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>Um livro foi solicitado - Sharebook</title>\r\n</head>\r\n<body>\r\n    <p>\r\n        Olá Cussa Mitre,\r\n    </p>\r\n    <p>\r\n        Um livro foi solicitado. Veja mais informações abaixo:\r\n    </p>\r\n\r\n    <ul>\r\n        <li><strong>Livro: </strong>Lord of the Rings</li>\r\n        <li><strong>Donatario: </strong>Walter Vinicius</li>\r\n        <li><strong>Linkedin Donatario:</strong>linkedin.com/walter</li>\r\n        <li><strong>Doador: </strong>Rodrigo</li>\r\n        <li><strong>Linkedin Doador:</strong>linkedin.com/rodrigo</li>\r\n    </ul>\r\n\r\n    <p>Sharebook</p>\r\n</body>\r\n</html>

            Assert.Contains("Olá Administrador(a),", result);
            Assert.Contains("<li><strong>Livro: </strong>Lord of the Rings</li>", result);
            Assert.Contains("<li><strong>Nome: </strong>Walter Vinicius</li>", result);
            Assert.Contains("<li><strong>Linkedin: </strong>linkedin.com/walter</li>", result);
            Assert.Contains("<li><strong>Telefone: </strong></li>", result);
            Assert.Contains("<li><strong>Email: </strong>walter@sharebook.com</li>", result);
            Assert.Contains("<li><strong>Nome: </strong>Rodrigo</li>", result);
            Assert.Contains("<li><strong>Linkedin: </strong>linkedin.com/rodrigo</li>", result);
            Assert.Contains("<li><strong>Telefone: </strong></li>", result);
            Assert.Contains("<li><strong>Email: </strong>rodrigo@sharebook.com</li>", result);
        }

        [Fact]
        public void VerifyEmailBookApprovedParse()
        {
            var result = emailTemplate.GenerateHtmlFromTemplateAsync("BookApprovedTemplate", book).Result;

            Assert.Contains("<title>Livro aprovado - Sharebook</title>", result);
            Assert.Contains("Olá Rodrigo", result);
            Assert.Contains("O livro Lord of the Rings foi aprovado e já está na nossa vitrine para doação.", result);
            Assert.Contains("<li><strong>Livro: </strong>Lord of the Rings</li>", result);
            Assert.Contains("<li><strong>Autor: </strong>J. R. R. Tolkien</li>", result);
        }

        [Fact]
        public void VerifyEmailContactUsNotificationParse()
        {

            var contactUs = new ContactUs()
            {
                Name = "Rafael Rocha",
                Email = "rafael.rochaoliveira@yahoo.com.br"
            };
          

            var result = emailTemplate.GenerateHtmlFromTemplateAsync("ContactUsNotificationTemplate", contactUs).Result;
            Assert.Contains("Olá, Rafael Rocha", result);

        }

        [Fact]
        public void VerifyEmailContactUsTemplateParse()
        {
            var result = emailTemplate.GenerateHtmlFromTemplateAsync("ContactUsTemplate", contactUs).Result;

            Assert.Contains("Olá, Administrador(a)!", result);
            Assert.Contains("Nome: Rafael Rocha", result);
            Assert.Contains("Email: rafael@sharebook.com.br", result);
            Assert.Contains("Telefone: (11) 954422-2765", result);
            Assert.Contains("Mensagem: At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident", result);

        }
    }
}