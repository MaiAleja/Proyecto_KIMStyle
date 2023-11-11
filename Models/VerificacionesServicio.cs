using KIM_Style.Data;
using KIM_Style.Migrations;
using KIM_Style.Models.DTO;
using KIM_Style.Models.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace KIM_Style.Models
{
    public class VerificacionesServicio
    {
        private readonly KIM_StyleContext _context;
        private readonly IEmailService _emailService;

        public VerificacionesServicio(KIM_StyleContext context)
        {
            this._context = context;
        }

        public VerificacionesServicio(KIM_StyleContext context, IEmailService emailService) {
            this._context = context;
            this._emailService = emailService;
        }
        public string verificacionUsuario (Usuario usuario,bool actualizar = false,string conservarContrasena = null)
        {
            string[] generos = { "Femenino", "Masculino", "No Binario" };
            if (usuario == null)
            {
                return "Información de usuario vacía";
            }
            if (string.IsNullOrWhiteSpace(usuario.nombres))
            {
                return "No se acepta el nombre vacío";
            }
            if (usuario.nombres.Trim().Length < 3)
            {
                return "El nombre es demasiado corto";
            }
            if (string.IsNullOrWhiteSpace(usuario.apellidos))
            {
                return "No se aceptan los apellidos vacíos";
            }
            if (usuario.apellidos.Trim().Length < 3)
            {
                return "Los Apellidos son demasiado cortos";
            }
            if (string.IsNullOrWhiteSpace(usuario.direccion))
            {
                return "No se acepta la dirección vacía";
            }
            if (usuario.direccion.Trim().Length < 8)
            {
                return "La Dirección es demasiada corta";
            }
            if (string.IsNullOrEmpty(usuario.correo))
            {
                return "El correo esta vacío";
            }
            if (!Regex.IsMatch(usuario.correo, @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$"))
            {
                return "Correo mal digitado";
            }
            if (conservarContrasena == null && string.IsNullOrEmpty(usuario.constrasena))
            {
                return "La contraseña esta vacía";
            }
            if (conservarContrasena == null && !Regex.IsMatch(usuario.constrasena, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$"))
            {
                return "Contraseña mal digitada";
            }
            if (!generos.Contains(usuario.genero))
            {
                return "El genero digitado no se encuentra en la lista";
            }
            if (string.IsNullOrEmpty(usuario.telefono) || usuario.telefono.Length < 10)
            {
                return "El telefono esta vacío o mal digitado";
            }
            if (usuario.cedula == 0)
            {
                return "Numero de cedula invalido";
            }
            if (_context.Usuario.FirstOrDefault(u => u.cedula == usuario.cedula) != null && !actualizar)
            {
                return "Numero de cedula ya registrado";
            }
            return null;
        }
        public string EncriptarContrasena(string contasena)
        {
            return BCrypt.Net.BCrypt.HashPassword(contasena);
        }
        public bool VerificarContrasena(string contrasena, string contrasenaEncriptada)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, contrasenaEncriptada);
        }

        public string VerificarLogin(string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrEmpty(contrasena))
            {
                return "Correo o Contrasena Incorrectos";
            }
            Usuario usuario = _context.Usuario.FirstOrDefault(u => u.correo == correo);
            if(usuario == null)
            {
                return "Correo o Contrasena Incorrectos";
            }
            if (!VerificarContrasena(contrasena, usuario.constrasena))
            {
                return "Correo o Contrasena Incorrectos";
            }
            return null;
        }

        public byte[] ConvertirAByteArray(IFormFile archivo)
        {
            using (var memoryStream = new MemoryStream())
            {
                archivo.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        internal string VerificarProducto(Producto producto)
        {
            string[] generos = { "Femenina", "Masculina", "Unisex" };

            if (producto == null)
            {
                return "El producto no tiene ningun dato";
            }
            if (string.IsNullOrWhiteSpace(producto.temporada))
            {
                return "El producto no tiene ninguna temporada establecida";
            }
            if (string.IsNullOrWhiteSpace(producto.material))
            {
                return "El producto no tiene ningun material establecida";
            }
            if (!generos.Contains(producto.genero))
            {
                return "El producto no especifica para que genero es";
            }
            if (producto.cantidad < 0)
            {
                return "Esa cantidad no puede ser asignada al producto";
            }
            if (producto.valor < 0)
            {
                return "Ese valor no puede ser asignada al producto";
            }
            if (producto.id_color <= 0)
            {
                return "No fue asignado ningun color para el producto";
            }
            if (producto.id_tipo_prenda <= 0)
            {
                return "No fue asignado ningun tipo de prenda para el producto";
            }
            if (producto.marca <= 0)
            {
                return "No fue asignada ninguna marca para el producto";
            }
            if (producto.talla <= 0)
            {
                return "No fue asignada ninguna talla para el producto";
            }
            return null;
        }

        public int EnviarCorreo(string correo)
        {
            int codigo = GenerarCodigo();

            EmailDTO request = new EmailDTO();
            request.Para = correo;
            request.Asunto = "Codigo de Verificacion - KIM Style";
            request.contenido = $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office""><head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""><meta http-equiv=""X-UA-Compatible"" content=""IE=edge""><meta name=""format-detection"" content=""telephone=no""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><title>Somos una tienda de ropa con productos de calidad</title><style type=""text/css"" emogrify=""no"">#outlook a {{ padding:0; }} .ExternalClass {{ width:100%; }} .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {{ line-height: 100%; }} table td {{ border-collapse: collapse; mso-line-height-rule: exactly; }} .editable.image {{ font-size: 0 !important; line-height: 0 !important; }} .nl2go_preheader {{ display: none !important; mso-hide:all !important; mso-line-height-rule: exactly; visibility: hidden !important; line-height: 0px !important; font-size: 0px !important; }} body {{ width:100% !important; -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; margin:0; padding:0; }} img {{ outline:none; text-decoration:none; -ms-interpolation-mode: bicubic; }} a img {{ border:none; }} table {{ border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt; }} th {{ font-weight: normal; text-align: left; }} *[class=""gmail-fix""] {{ display: none !important; }} </style><style type=""text/css"" emogrify=""no""> @media (max-width: 600px) {{ .gmx-killpill {{ content: ' \03D1';}} }} </style><style type=""text/css"" emogrify=""no"">@media (max-width: 600px) {{ .gmx-killpill {{ content: ' \03D1';}} .r0-o {{ border-style: solid !important; margin: 0 auto 0 0 !important; width: 100% !important }} .r1-i {{ background-color: #ffffff !important }} .r2-c {{ box-sizing: border-box !important; text-align: center !important; valign: top !important; width: 100% !important }} .r3-o {{ border-style: solid !important; margin: 0 auto 0 auto !important; width: 100% !important }} .r4-i {{ padding-left: 0px !important; padding-right: 0px !important; padding-top: 20px !important }} .r5-c {{ box-sizing: border-box !important; display: block !important; valign: top !important; width: 100% !important }} .r6-o {{ border-style: solid !important; width: 100% !important }} .r7-c {{ box-sizing: border-box !important; text-align: left !important; valign: top !important; width: 100% !important }} .r8-i {{ text-align: center !important }} .r9-i {{ padding-top: 15px !important; text-align: center !important }} .r10-c {{ box-sizing: border-box !important; padding-top: 10px !important; text-align: center !important; valign: top !important; width: 100% !important }} .r11-c {{ box-sizing: border-box !important; text-align: center !important; width: 100% !important }} .r12-i {{ padding-bottom: 10px !important; padding-top: 10px !important }} .r13-i {{ background-color: #61288a !important; padding-bottom: 15px !important; padding-left: 0px !important; padding-right: 0px !important; padding-top: 15px !important; text-align: left !important }} .r14-o {{ background-size: auto !important; border-style: solid !important; margin: 0 auto 0 auto !important; width: 100% !important }} .r15-i {{ padding-bottom: 15px !important; padding-top: 15px !important }} body {{ -webkit-text-size-adjust: none }} .nl2go-responsive-hide {{ display: none }} .nl2go-body-table {{ min-width: unset !important }} .mobshow {{ height: auto !important; overflow: visible !important; max-height: unset !important; visibility: visible !important; border: none !important }} .resp-table {{ display: inline-table !important }} .magic-resp {{ display: table-cell !important }} }} </style><style type=""text/css"">p, h1, h2, h3, h4, ol, ul {{ margin: 0; }} a, a:link {{ color: #696969; text-decoration: underline }} .nl2go-default-textstyle {{ color: #3b3f44; font-family: arial,helvetica,sans-serif; font-size: 16px; line-height: 1.5; word-break: break-word }} .default-button {{ color: #ffffff; font-family: arial,helvetica,sans-serif; font-size: 16px; font-style: normal; font-weight: normal; line-height: 1.15; text-decoration: none; word-break: break-word }} .default-heading1 {{ color: #1F2D3D; font-family: arial,helvetica,sans-serif; font-size: 36px; word-break: break-word }} .default-heading2 {{ color: #1F2D3D; font-family: arial,helvetica,sans-serif; font-size: 32px; word-break: break-word }} .default-heading3 {{ color: #1F2D3D; font-family: arial,helvetica,sans-serif; font-size: 24px; word-break: break-word }} .default-heading4 {{ color: #1F2D3D; font-family: arial,helvetica,sans-serif; font-size: 18px; word-break: break-word }} a[x-apple-data-detectors] {{ color: inherit !important; text-decoration: inherit !important; font-size: inherit !important; font-family: inherit !important; font-weight: inherit !important; line-height: inherit !important; }} .no-show-for-you {{ border: none; display: none; float: none; font-size: 0; height: 0; line-height: 0; max-height: 0; mso-hide: all; overflow: hidden; table-layout: fixed; visibility: hidden; width: 0; }} </style><!--[if mso]><xml> <o:OfficeDocumentSettings> <o:AllowPNG/> <o:PixelsPerInch>96</o:PixelsPerInch> </o:OfficeDocumentSettings> </xml><![endif]--></head><body bgcolor=""#ffffff"" text=""#3b3f44"" link=""#696969"" yahoo=""fix"" style=""background-color: #ffffff;""> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" class=""nl2go-body-table"" width=""100%"" style=""background-color: #ffffff; width: 100%;""><tr><td> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" width=""100%"" align=""left"" class=""r0-o"" style=""table-layout: fixed; width: 100%;""><tr><td valign=""top"" class=""r1-i"" style=""background-color: #ffffff;""> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" width=""100%"" align=""center"" class=""r3-o"" style=""table-layout: fixed; width: 100%;""><tr><td class=""r4-i"" style=""padding-top: 20px;""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation""><tr><th width=""100%"" valign=""top"" class=""r5-c"" style=""font-weight: normal;""> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" width=""100%"" align=""left"" class=""r0-o"" style=""table-layout: fixed; width: 100%;""><tr><td valign=""top""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation""><tr><td class=""r7-c"" align=""left""> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" width=""100%"" class=""r0-o"" style=""table-layout: fixed; width: 100%;""><tr><td align=""center"" valign=""top"" class=""r8-i nl2go-default-textstyle"" style=""color: #3b3f44; font-family: arial,helvetica,sans-serif; font-size: 16px; line-height: 1.5; word-break: break-word; text-align: center;""> <div><h1 class=""default-heading1"" style=""margin: 0; color: #1f2d3d; font-family: arial,helvetica,sans-serif; font-size: 36px; word-break: break-word;""><span style=""font-family: Arial; font-size: 36px;""><strong>Código de Recuperación</strong></span></h1></div> </td> </tr></table></td> </tr><tr><td class=""r7-c"" align=""left""> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" width=""100%"" class=""r0-o"" style=""table-layout: fixed; width: 100%;""><tr><td align=""center"" valign=""top"" class=""r9-i nl2go-default-textstyle"" style=""color: #3b3f44; font-family: arial,helvetica,sans-serif; font-size: 16px; word-break: break-word; line-height: 1.15; padding-top: 15px; text-align: center;""> <div><p style=""margin: 0;"">Para continuar con el proceso de recuperación de su contraseña digite el siguiente Código en la pagina web de Kim Style</p><p style=""margin: 0;""> </p></div> </td> </tr></table></td> </tr><tr><td class=""r10-c nl2go-default-textstyle"" align=""center"" style=""color: #3b3f44; font-family: arial,helvetica,sans-serif; font-size: 16px; word-break: break-word; line-height: 1.15; padding-top: 10px; text-align: center; valign: top;""> <div><p style=""margin: 0;""><span style=""color: #61288A; font-family: 'Comic sans ms', cursive; font-size: 32px;""><strong>{codigo}</strong></span></p><p style=""margin: 0;""> </p></div> </td> </tr><tr><td class=""r11-c"" align=""center""> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" width=""100%"" class=""r3-o"" style=""table-layout: fixed;""><tr><td class=""r12-i"" style=""padding-bottom: 10px; padding-top: 10px; height: 3px;""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation""><tr><td><table width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" valign="""" class=""r12-i"" height=""3"" style=""border-top-style: dashed; background-clip: border-box; border-top-color: #61288a; border-top-width: 3px; font-size: 3px; line-height: 3px;""><tr><td height=""0"" style=""font-size: 0px; line-height: 0px;"">­</td> </tr></table></td> </tr></table></td> </tr></table></td> </tr><tr><td class=""r2-c"" align=""center""> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" width=""90%"" class=""r3-o"" style=""border-collapse: separate; border-radius: 6px; table-layout: fixed; width: 90%;""><tr><td align=""left"" valign=""top"" class=""r13-i nl2go-default-textstyle"" style=""color: #3b3f44; font-family: arial,helvetica,sans-serif; font-size: 16px; line-height: 1.5; word-break: break-word; background-color: #61288a; border-radius: 6px; padding-bottom: 15px; padding-top: 15px; text-align: left;""> <div><h4 class=""default-heading4"" style=""margin: 0; color: #1f2d3d; font-family: arial,helvetica,sans-serif; font-size: 18px; word-break: break-word; margin-left: 40px;""><span style=""color: #F9F7F7; font-family: Arial; font-size: 18px;"">Atentamente: Equipo de Kim Style </span></h4></div> </td> </tr></table></td> </tr><tr><td class=""r2-c"" align=""center""> <table cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" width=""49%"" class=""r14-o"" style=""table-layout: fixed; width: 49%;""><tr><td class=""r15-i"" style=""font-size: 0px; line-height: 0px; padding-bottom: 15px; padding-top: 15px;""> <img src=""https://img.mailinblue.com/6728272/images/content_library/original/654647ff46515b1c596e5a6c.png"" width="""" border=""0"" style=""display: block; width: 100%;""></td> </tr></table></td> </tr></table></td> </tr></table></th> </tr></table></td> </tr></table></td> </tr></table></td> </tr></table></body></html>";

            _emailService.SendEmail(request);
            return codigo;
        }

        public static int GenerarCodigo()
        {
            Random random = new Random(); 
            int min = 100000;
            int max = 999999;

            return random.Next(min, max + 1);
        }

        public bool ValidarContrasena(string contrasena)
        {
            if (string.IsNullOrEmpty(contrasena))
            {
                return false;
            }
            if (!Regex.IsMatch(contrasena, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$"))
            {
                return false;
            }
            return true;
        }
    }
}
