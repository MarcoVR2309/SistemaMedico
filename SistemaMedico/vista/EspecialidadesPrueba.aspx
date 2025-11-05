<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EspecialidadesPrueba.aspx.cs" Inherits="SistemaMedico.vista.EspecialidadesPrueba" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Prueba de CRUD de Especialidades</title>
    <style>
        /* Estilos para la demostración */
        .form-group { margin-bottom: 15px; }
        .form-group label { display: block; font-weight: bold; margin-bottom: 5px; }
        .form-group input[type="text"], .form-group textarea { width: 98%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
        .btn-add { background-color: #4CAF50; color: white; padding: 10px 15px; border: none; border-radius: 4px; cursor: pointer; }
        .table-bordered { border-collapse: collapse; width: 100%; margin-top: 20px; }
        .table-bordered th, .table-bordered td { border: 1px solid #ddd; padding: 8px; text-align: left; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="font-family: Arial, sans-serif; padding: 20px;">
            <h1>Prueba de CRUD: Especialidades</h1>
            <hr />

            <h2>1. Agregar Nueva Especialidad (CREATE)</h2>
            <div class="form-group">
                <label for="<%= txtNombre.ClientID %>">Nombre Corto:</label>
                <asp:TextBox ID="txtNombre" runat="server"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="<%= txtDescripcion.ClientID %>">Descripción:</label>
                <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Button ID="btnAgregar" runat="server" Text="Agregar Especialidad" CssClass="btn-add" OnClick="btnAgregar_Click" />
            </div>

            <hr />
            
            <h2>2. Listado de Especialidades (READ)</h2>
            <asp:Label ID="lblEstado" runat="server" Font-Bold="True"></asp:Label>
            
            <br />       
            <asp:GridView ID="gvEspecialidades" runat="server" AutoGenerateColumns="False" CssClass="table-bordered">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="ID" />
                    <asp:BoundField DataField="NomEsp" HeaderText="Nombre Corto" />
                    <asp:BoundField DataField="DesEsp" HeaderText="Descripción" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
