<%@ Page Title="" Language="C#" MasterPageFile="~/mpFormatada.master" AutoEventWireup="true"
    CodeBehind="CadastroCarteiras.aspx.cs" Inherits="CadastroCarteiras" %>

<%@ Register Src="~/UserControl/Unidade.ascx" TagPrefix="uc1" TagName="Unidade" %>
<%@ Register Src="~/UserControl/Profissionais.ascx" TagPrefix="uc2" TagName="Profissionais" %>
<%@ Register Assembly="DevExpress.Web.v12.2" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v12.2" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v12.2, Version=12.2.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView.Export" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v12.2, Version=12.2.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeadFormatada" runat="server">

    <style type="text/css">
        .margin-checkbox input{
            margin: 5px 5px 0px 0px !important;
        }
    </style>

    <script type="text/javascript">
        function Mascara(o, f) {
            v_obj = o;
            v_fun = f;
            setTimeout("execmascara()", 1);
        }

        function Limite(v) {
            v = v.replace(/\D/g, "");
            v = v.replace(/([0-9]{2})$/g, ",$1");
            if (v.length > 6)
                v = v.replace(/([0-9]{3}),([0-9]{2}$)/g, ".$1,$2");
            return v;
        }
        function LimiteQuantidade(v) {
            v = v.replace(/\D/g, "");
            v = v.replace(/(\d)(\d{5})$/, "$1.$2");
            v = v.replace(/(\d)(\d{3})$/, "$1.$2");

            return v;
        }


        function execmascara() {
            v_obj.value = v_fun(v_obj.value);
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function ShowModalDeleteConfirmation(idProfissional, tipo) {
            _Comum.ConfirmNoAction(" Confirma a exclusão do Profissional? ", "Remover(\"" + idProfissional + "\",\"" + tipo + "\")");
        }

        function Remover(ID, tipo) {
            $("#<% = hdfIdProfissionalRemocao.ClientID %>").val(ID);
            switch (tipo) {
                case "M": $("#<% = btnDeleteMedicos.ClientID %>").click();
                    break;
                case "E": $("#<% = btnDeleteEnfermeiros.ClientID %>").click();
                    break;
                case "A": $("#<% = btnDeleteAgentes.ClientID %>").click();
                    break;
            }
        }

        function Editar(ID, tipo) {
            $("#<% = hdfIdProfissionalRemocao.ClientID %>").val(ID);
            switch (tipo) {
                case "M": $("#<% = btnEditMedicos.ClientID %>").click();
                    break;
                case "E": $("#<% = btnEditEnfermeiros.ClientID %>").click();
                    break;
                case "A": $("#<% = btnEditAgentes.ClientID %>").click();
                    break;
            }
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyFormatada" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <asp:HiddenField ID="hdfIdProfissionalRemocao" runat="server" ClientIDMode="Static" />

            <div class="BoxConteudoPagina Box98 FloatEsquerdo">
                <div class="BoxFull SitePath">
                    <asp:HyperLink ID="hplSiteMapHome" CssClass="SiteMap" runat="server" EnableViewState="False" NavigateUrl="Menu.aspx">Home</asp:HyperLink>
                    <asp:Label ID="Label33" runat="server" EnableViewState="False" Text=" >> "></asp:Label>
                    <asp:Label ID="Label34" CssClass="SitePathAtual" runat="server" EnableViewState="False" Text="Cadastro de Carteira"></asp:Label>
                </div>
                <div class="LinhaTitulo">
                    <asp:Label ID="lblTitulo" runat="server" Text="Cadastro de Carteira" EnableViewState="False"></asp:Label>
                </div>
                <div class="LinhaSeparacao"></div>

                <div class="BoxFull" style="margin-bottom: 20px;">
                    <fieldset class="fieldset" style="border: 1px solid #ccc; padding: 0 10px 10px 10px;">
                        <legend>Tipo de Carteira:</legend>
                        <div class="BoxFull FloatEsquerdo">
                            <asp:DropDownList ID="ddlTipoCarteira" CssClass="Edit" Width="100%" AutoPostBack="true"
                                runat="server" OnSelectedIndexChanged="ddlTipoCarteira_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </fieldset>

                    <asp:Panel ID="pnDados" CssClass="BoxFull FloatEsquerdo" runat="server">
                        <div class="BoxFull">
                            <uc1:Unidade runat="server" ID="Unidade" AtualizarGrade="true" Legenda="Centro Médico" ValidaUsuarioLogado="true" />
                        </div>
                        <fieldset class="fieldset" id="fieldSetEquipe" style="border: 1px solid #ccc; padding: 0 10px 10px 10px;">
                            <legend id="legendEquipeGrupo" runat="server">Equipe:</legend>
                            <div class="BoxFull FloatEsquerdo">
                                <asp:DropDownList ID="ddlEquipe" CssClass="Edit" Width="100%" AutoPostBack="true"
                                    runat="server" OnSelectedIndexChanged="ddlEquipe_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </fieldset>
                    </asp:Panel>

                    <fieldset runat="server" class="fieldset" id="fieldPro" style="border: 1px solid #ccc; padding: 0 10px 10px 10px;">
                        <legend>Profissionais:</legend>

                        <asp:Panel ID="pnMedicos" CssClass="BoxFull FloatEsquerdo" runat="server">
                            <fieldset class="fieldset" style="border: 1px solid #ccc; padding: 0 10px 10px 10px;">
                                <legend>Médicos:</legend>
                                <div class="BoxFull" style="text-align: left; margin-bottom: 2px;">
                                    <asp:Button ID="Medico" runat="server" CssClass="BotaoIncluirNovo" OnClick="hplIncluir_Click" Text="Incluir" ToolTip="Incluir" CommandName="CRM" />
                                </div>

                                <asp:ObjectDataSource
                                    ID="odsDadosM"
                                    runat="server"
                                    OnSelected="odsDados_Selected"
                                    SelectMethod="BuscaProfissionalM"
                                    TypeName="CadastroCarteiras"></asp:ObjectDataSource>

                                <asp:UpdatePanel ID="upGradeM" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <dx:ASPxGridViewExporter ID="GradeExportM" runat="server" GridViewID="GradeMedicos">
                                        </dx:ASPxGridViewExporter>
                                        <dx:ASPxGridView
                                            ID="GradeMedicos"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            ClientIDMode="AutoID"
                                            CssFilePath="~/App_Themes/PlasticBlue/{0}/styles.css"
                                            CssPostfix="PlasticBlue"
                                            DataSourceID="odsDadosM"
                                            KeyFieldName="ProfissionalID"
                                            OnHtmlRowCreated="GradeMedicos_HtmlRowCreated"
                                            OnPageIndexChanged="GradeMedicos_PageIndexChanged"
                                            Width="100%">
                                            <BorderBottom BorderStyle="None" />
                                            <Styles CssFilePath="~/App_Themes/PlasticBlue/{0}/styles.css" CssPostfix="PlasticBlue">
                                                <Header ImageSpacing="10px" SortingImageSpacing="10px" />
                                                <AlternatingRow Enabled="True">
                                                </AlternatingRow>
                                                <Cell>
                                                    <Paddings Padding="3px" />
                                                </Cell>
                                            </Styles>
                                            <SettingsPager CurrentPageNumberFormat="{0}" PageSize="10" ShowDefaultImages="False">
                                                <AllButton Text="Todos">
                                                </AllButton>
                                                <NextPageButton Text="Próxima &gt;">
                                                </NextPageButton>
                                                <PrevPageButton Text="&lt; Anterior">
                                                </PrevPageButton>
                                                <Summary AllPagesText="Páginas: {0} - {1} ({2} items)"
                                                    Text="Página {0} de {1} ({2} itens)" />
                                            </SettingsPager>
                                            <Columns>
                                                <dx:GridViewDataTextColumn Caption="Profissional" FieldName="Nome" VisibleIndex="0" Width="27%"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Conselho" FieldName="SiglaNumeroProfissional" VisibleIndex="0" Width="12%"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Substituto" FieldName="NomeSubstituto" VisibleIndex="1" Width="25%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Conselho (Substituto)" FieldName="SiglaNumeroSubstituto" VisibleIndex="2" Width="15%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Limite" FieldName="LimiteTotalCarteira" VisibleIndex="3" Width="7%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Apraz." FieldName="AprazamentoDescricao" VisibleIndex="4" Width="7%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Ações" FieldName="" VisibleIndex="5" Width="7%">
                                                    <DataItemTemplate>
                                                        <asp:HyperLink ID="hplEditar" runat="server" ToolTip="Alterar">
                                                            <img src="img/editar.png" alt="Editar registro"/>
                                                        </asp:HyperLink>
                                                        <asp:HyperLink ID="hplInativar" runat="server" ToolTip="Excluir um registo">
                                                            <img src="img/excluir.png" alt="Excluir um registo"/>
                                                        </asp:HyperLink>
                                                    </DataItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <CellStyle HorizontalAlign="Center" />
                                                </dx:GridViewDataTextColumn>
                                            </Columns>
                                            <ImagesFilterControl>
                                                <LoadingPanel Url="img/ajax-loader-gde.gif">
                                                </LoadingPanel>
                                            </ImagesFilterControl>
                                            <Images SpriteCssFilePath="~/App_Themes/PlasticBlue/{0}/sprite.css">
                                                <LoadingPanelOnStatusBar Url="~/App_Themes/PlasticBlue/GridView/gvLoadingOnStatusBar.gif">
                                                </LoadingPanelOnStatusBar>
                                                <LoadingPanel Url="img/ajax-loader-gde.gif">
                                                </LoadingPanel>
                                            </Images>
                                            <Border BorderStyle="None" />
                                            <SettingsText EmptyDataRow="Nenhum registro foi encontrado"
                                                GroupContinuedOnNextPage="(continua na próxima página)"
                                                GroupPanel="Arraste uma coluna aqui para agrupar as informações" />
                                            <BorderRight BorderStyle="None" />
                                            <BorderTop BorderStyle="None" />
                                            <BorderLeft BorderStyle="None" />
                                            <Settings GridLines="Vertical" ShowGroupPanel="False" />
                                            <SettingsLoadingPanel Text="Selecionando os dados." />
                                            <StylesEditors>
                                                <CalendarHeader Spacing="11px">
                                                </CalendarHeader>
                                                <ProgressBar Height="25px">
                                                </ProgressBar>
                                            </StylesEditors>
                                        </dx:ASPxGridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <asp:Button ID="btnEditMedicos" CommandName="CRM" ClientIDMode="Static" CssClass="Escondido" runat="server" Text="btnEdit (escondido)" OnClick="btnEdit_Click" />
                                <asp:Button ID="btnDeleteMedicos" ClientIDMode="Static" CssClass="Escondido" runat="server" Text="btnDelete (escondido)" OnClick="btnDelete_Click" />

                                <div class="BoxFull" style="padding: 15px 0 5px 0;">
                                    Total de Médicos:
                                    <asp:Label runat="server" ID="totalMedicos" />
                                </div>
                            </fieldset>
                        </asp:Panel>

                        <asp:Panel ID="pnEnfermeiros" CssClass="BoxFull FloatEsquerdo" runat="server">
                            <fieldset class="fieldset" style="border: 1px solid #ccc; padding: 0 10px 10px 10px;">
                                <legend>Enfermeiros:</legend>
                                <div class="BoxFull" style="text-align: left; margin-bottom: 2px;">
                                    <asp:Button ID="Enfermeiro" runat="server" CssClass="BotaoIncluirNovo" OnClick="hplIncluir_Click" Text="Incluir" ToolTip="Incluir" CommandName="COREN" />
                                </div>

                                <asp:ObjectDataSource
                                    ID="odsDadosE"
                                    runat="server"
                                    OnSelected="odsDados_Selected"
                                    SelectMethod="BuscaProfissionalE"
                                    TypeName="CadastroCarteiras"></asp:ObjectDataSource>

                                <asp:UpdatePanel ID="upGradeE" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <dx:ASPxGridViewExporter ID="GradeExportE" runat="server" GridViewID="GradeEnfermeiros">
                                        </dx:ASPxGridViewExporter>
                                        <dx:ASPxGridView
                                            ID="GradeEnfermeiros"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            ClientIDMode="AutoID"
                                            CssFilePath="~/App_Themes/PlasticBlue/{0}/styles.css"
                                            CssPostfix="PlasticBlue"
                                            DataSourceID="odsDadosE"
                                            KeyFieldName="ProfissionalID"
                                            OnHtmlRowCreated="GradeEnfermeiros_HtmlRowCreated"
                                            OnPageIndexChanged="GradeEnfermeiros_PageIndexChanged"
                                            Width="100%">
                                            <BorderBottom BorderStyle="None" />
                                            <Styles CssFilePath="~/App_Themes/PlasticBlue/{0}/styles.css" CssPostfix="PlasticBlue">
                                                <Header ImageSpacing="10px" SortingImageSpacing="10px" />
                                                <AlternatingRow Enabled="True">
                                                </AlternatingRow>
                                                <Cell>
                                                    <Paddings Padding="3px" />
                                                </Cell>
                                            </Styles>
                                            <SettingsPager CurrentPageNumberFormat="{0}" PageSize="10" ShowDefaultImages="False">
                                                <AllButton Text="Todos">
                                                </AllButton>
                                                <NextPageButton Text="Próxima &gt;">
                                                </NextPageButton>
                                                <PrevPageButton Text="&lt; Anterior">
                                                </PrevPageButton>
                                                <Summary AllPagesText="Páginas: {0} - {1} ({2} items)"
                                                    Text="Página {0} de {1} ({2} itens)" />
                                            </SettingsPager>
                                            <Columns>
                                                <dx:GridViewDataTextColumn Caption="Profissional" FieldName="Nome" VisibleIndex="0" Width="27%"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Conselho" FieldName="SiglaNumeroProfissional" VisibleIndex="0" Width="12%"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Substituto" FieldName="NomeSubstituto" VisibleIndex="1" Width="25%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Conselho (Substituto)" FieldName="SiglaNumeroSubstituto" VisibleIndex="2" Width="15%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Limite" FieldName="LimiteTotalCarteira" VisibleIndex="3" Width="7%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Apraz." FieldName="AprazamentoDescricao" VisibleIndex="4" Width="7%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Ações" FieldName="" VisibleIndex="5" Width="7%">
                                                    <DataItemTemplate>
                                                        <asp:HyperLink ID="hplEditar" runat="server" ToolTip="Alterar">
                                                            <img src="img/editar.png" alt="Editar registro"/>
                                                        </asp:HyperLink>
                                                        <asp:HyperLink ID="hplInativar" runat="server" ToolTip="Excluir um registo">
                                                            <img src="img/excluir.png" alt="Excluir um registo"/>
                                                        </asp:HyperLink>
                                                    </DataItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <CellStyle HorizontalAlign="Center" />
                                                </dx:GridViewDataTextColumn>
                                            </Columns>
                                            <ImagesFilterControl>
                                                <LoadingPanel Url="img/ajax-loader-gde.gif">
                                                </LoadingPanel>
                                            </ImagesFilterControl>
                                            <Images SpriteCssFilePath="~/App_Themes/PlasticBlue/{0}/sprite.css">
                                                <LoadingPanelOnStatusBar Url="~/App_Themes/PlasticBlue/GridView/gvLoadingOnStatusBar.gif">
                                                </LoadingPanelOnStatusBar>
                                                <LoadingPanel Url="img/ajax-loader-gde.gif">
                                                </LoadingPanel>
                                            </Images>
                                            <Border BorderStyle="None" />
                                            <SettingsText EmptyDataRow="Nenhum registro foi encontrado"
                                                GroupContinuedOnNextPage="(continua na próxima página)"
                                                GroupPanel="Arraste uma coluna aqui para agrupar as informações" />
                                            <BorderRight BorderStyle="None" />
                                            <BorderTop BorderStyle="None" />
                                            <BorderLeft BorderStyle="None" />
                                            <Settings GridLines="Vertical" ShowGroupPanel="False" />
                                            <SettingsLoadingPanel Text="Selecionando os dados." />
                                            <StylesEditors>
                                                <CalendarHeader Spacing="11px">
                                                </CalendarHeader>
                                                <ProgressBar Height="25px">
                                                </ProgressBar>
                                            </StylesEditors>
                                        </dx:ASPxGridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <asp:Button ID="btnEditEnfermeiros" CommandName="COREN" ClientIDMode="Static" CssClass="Escondido" runat="server" Text="btnEdit (escondido)" OnClick="btnEdit_Click" />
                                <asp:Button ID="btnDeleteEnfermeiros" ClientIDMode="Static" CssClass="Escondido" runat="server" Text="btnDelete (escondido)" OnClick="btnDelete_Click" />

                                <div class="BoxFull" style="padding: 15px 0 5px 0;">
                                    Total de Enfermeiros:
                                    <asp:Label runat="server" ID="totalEnfermeiros" />
                                </div>
                            </fieldset>
                        </asp:Panel>

                        <asp:Panel ID="pnAgentes" CssClass="BoxFull FloatEsquerdo" runat="server">
                            <fieldset class="fieldset" style="border: 1px solid #ccc; padding: 0 10px 10px 10px;">
                                <legend>Agentes:</legend>
                                <div class="BoxFull" style="text-align: left; margin-bottom: 2px;">
                                    <asp:Button ID="Agente" runat="server" CssClass="BotaoIncluirNovo" OnClick="hplIncluir_Click" Text="Incluir" ToolTip="Incluir" CommandName="TODOS" />
                                </div>

                                <asp:ObjectDataSource
                                    ID="odsDadosA"
                                    runat="server"
                                    OnSelected="odsDados_Selected"
                                    SelectMethod="BuscaProfissionalA"
                                    TypeName="CadastroCarteiras"></asp:ObjectDataSource>

                                <asp:UpdatePanel ID="upGradeA" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <dx:ASPxGridViewExporter ID="GradeExportA" runat="server" GridViewID="GradeAgentes">
                                        </dx:ASPxGridViewExporter>
                                        <dx:ASPxGridView
                                            ID="GradeAgentes"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            ClientIDMode="AutoID"
                                            CssFilePath="~/App_Themes/PlasticBlue/{0}/styles.css"
                                            CssPostfix="PlasticBlue"
                                            DataSourceID="odsDadosA"
                                            KeyFieldName="ProfissionalID"
                                            OnHtmlRowCreated="GradeAgentes_HtmlRowCreated"
                                            OnPageIndexChanged="GradeAgentes_PageIndexChanged"
                                            Width="100%">
                                            <BorderBottom BorderStyle="None" />
                                            <Styles CssFilePath="~/App_Themes/PlasticBlue/{0}/styles.css" CssPostfix="PlasticBlue">
                                                <Header ImageSpacing="10px" SortingImageSpacing="10px" />
                                                <AlternatingRow Enabled="True">
                                                </AlternatingRow>
                                                <Cell>
                                                    <Paddings Padding="3px" />
                                                </Cell>
                                            </Styles>
                                            <SettingsPager CurrentPageNumberFormat="{0}" PageSize="10" ShowDefaultImages="False">
                                                <AllButton Text="Todos">
                                                </AllButton>
                                                <NextPageButton Text="Próxima &gt;">
                                                </NextPageButton>
                                                <PrevPageButton Text="&lt; Anterior">
                                                </PrevPageButton>
                                                <Summary AllPagesText="Páginas: {0} - {1} ({2} items)"
                                                    Text="Página {0} de {1} ({2} itens)" />
                                            </SettingsPager>
                                            <Columns>
                                                <dx:GridViewDataTextColumn Caption="Profissional" FieldName="Nome" VisibleIndex="0" Width="27%"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Conselho" FieldName="SiglaNumeroProfissional" VisibleIndex="0" Width="12%"></dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Substituto" FieldName="NomeSubstituto" VisibleIndex="1" Width="25%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Conselho (Substituto)" FieldName="SiglaNumeroSubstituto" VisibleIndex="2" Width="15%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Limite" FieldName="LimiteTotalCarteira" VisibleIndex="3" Width="7%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Apraz." FieldName="AprazamentoDescricao" VisibleIndex="4" Width="7%">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <CellStyle HorizontalAlign="Left" />
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn Caption="Ações" FieldName="" VisibleIndex="5" Width="7%">
                                                    <DataItemTemplate>
                                                        <asp:HyperLink ID="hplEditar" runat="server" ToolTip="Alterar">
                                                            <img src="img/editar.png" alt="Editar registro"/>
                                                        </asp:HyperLink>
                                                        <asp:HyperLink ID="hplInativar" runat="server" ToolTip="Excluir um registo">
                                                            <img src="img/excluir.png" alt="Excluir um registo"/>
                                                        </asp:HyperLink>
                                                    </DataItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <CellStyle HorizontalAlign="Center" />
                                                </dx:GridViewDataTextColumn>
                                            </Columns>
                                            <ImagesFilterControl>
                                                <LoadingPanel Url="img/ajax-loader-gde.gif">
                                                </LoadingPanel>
                                            </ImagesFilterControl>
                                            <Images SpriteCssFilePath="~/App_Themes/PlasticBlue/{0}/sprite.css">
                                                <LoadingPanelOnStatusBar Url="~/App_Themes/PlasticBlue/GridView/gvLoadingOnStatusBar.gif">
                                                </LoadingPanelOnStatusBar>
                                                <LoadingPanel Url="img/ajax-loader-gde.gif">
                                                </LoadingPanel>
                                            </Images>
                                            <Border BorderStyle="None" />
                                            <SettingsText EmptyDataRow="Nenhum registro foi encontrado"
                                                GroupContinuedOnNextPage="(continua na próxima página)"
                                                GroupPanel="Arraste uma coluna aqui para agrupar as informações" />
                                            <BorderRight BorderStyle="None" />
                                            <BorderTop BorderStyle="None" />
                                            <BorderLeft BorderStyle="None" />
                                            <Settings GridLines="Vertical" ShowGroupPanel="False" />
                                            <SettingsLoadingPanel Text="Selecionando os dados." />
                                            <StylesEditors>
                                                <CalendarHeader Spacing="11px">
                                                </CalendarHeader>
                                                <ProgressBar Height="25px">
                                                </ProgressBar>
                                            </StylesEditors>
                                        </dx:ASPxGridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <asp:Button ID="btnEditAgentes" CommandName="TODOS" ClientIDMode="Static" CssClass="Escondido" runat="server" Text="btnEdit (escondido)" OnClick="btnEdit_Click" />
                                <asp:Button ID="btnDeleteAgentes" ClientIDMode="Static" CssClass="Escondido" runat="server" Text="btnDelete (escondido)" OnClick="btnDelete_Click" />

                                <div class="BoxFull" style="padding: 15px 0 5px 0;">
                                    Total de Agentes:
                                    <asp:Label runat="server" ID="totalAgentes" />
                                </div>
                            </fieldset>
                        </asp:Panel>

                    </fieldset>

                    <ajaxToolkit:ModalPopupExtender
                        ID="modalProfissional"
                        DropShadow="false" runat="server"
                        PopupControlID="pnlAdicionarModalProfissional"
                        TargetControlID="btnshowpopupFake"
                        CancelControlID="btnshowpopupFake"
                        BackgroundCssClass="modalBackground">
                    </ajaxToolkit:ModalPopupExtender>

                    <asp:Panel ID="pnlAdicionarModalProfissional" Width="452px" Height="292px" runat="server" CssClass="modalPopup" Style="display: none;" DefaultButton="btnAtualizarGrade">
                        <fieldset class="fieldset" style="border: 1px solid #ccc; height: 280px; padding: 5px; width: 440px;">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="Box99 FloatEsquerdo" style="height: 240px; width: 440px;">
                                        <div class="ui-datepicker-header ui-widget-header ui-helper-clearfix ui-corner-all TituloPopup">
                                            <asp:Label ID="TituloPopUp" runat="server" EnableViewState="true"></asp:Label>
                                        </div>
                                        <div class="LinhaSeparacaoPeq"></div>

                                        <div class="Box95">
                                            <div class="LinhaSeparacaoPeq"></div>

                                            <fieldset class="fieldset" style="border: 1px solid #ccc; padding-left: 10px; margin-left: 20px;">
                                                <legend id="legenda" runat="server" style="background: transparent; color: #005194;">Profissional</legend>
                                                <div class="Box25 FloatEsquerdo">
                                                    <asp:TextBox Visible="false" ID="txtIdProfissionalPrincipal" CssClass="Edit" runat="server" Width="90%" Enabled="false" ViewStateMode="Enabled" EnableViewState="true"></asp:TextBox>
                                                    <asp:TextBox ID="txtConselhoProfissional" CssClass="Edit" runat="server" Width="90%"
                                                        ViewStateMode="Enabled" EnableViewState="true" AutoPostBack="true" OnTextChanged="txtConselhoProfissional_TextChanged"></asp:TextBox>
                                                </div>
                                                <div class="Box70 FloatEsquerdo">
                                                    <asp:TextBox ID="txtNomeProfissionalPrincipal" CssClass="Edit" runat="server" Width="95%" Enabled="false" ViewStateMode="Enabled" EnableViewState="true"></asp:TextBox>
                                                </div>
                                                <div class="Box5 FloatEsquerdo" style="padding-top: 5px;">
                                                    <asp:ImageButton ID="btnPesquisaProfissionalPrincipal" ToolTip="Pesquisar profissional principal" OnClick="btnPesquisaProfissionalPrincipal_Click" runat="server" ImageUrl="~/img/lupa.png" CommandName="ProfissionalPrincipal" />
                                                </div>

                                                <div class="BoxFull FloatEsquerdo" style="padding: 10px 0 5px 0;">
                                                    <asp:Label ID="labelSub" Text="Substituto" runat="server" />
                                                </div>
                                                <div class="Box25 FloatEsquerdo">
                                                    <asp:TextBox Visible="false" ID="txtIdProfissionalSubstituto" CssClass="Edit" runat="server" Width="90%" Enabled="false" ViewStateMode="Enabled" EnableViewState="true"></asp:TextBox>
                                                    <asp:TextBox ID="txtConselhoProfissionalSubstituto" CssClass="Edit" runat="server" Width="90%"
                                                        ViewStateMode="Enabled" EnableViewState="true" AutoPostBack="true" OnTextChanged="txtConselhoProfissionalSubstituto_TextChanged"></asp:TextBox>
                                                </div>
                                                <div class="Box70 FloatEsquerdo">
                                                    <asp:TextBox ID="txtNomeProfissionalSubstituto" CssClass="Edit" runat="server" Width="95%" Enabled="false" ViewStateMode="Enabled" EnableViewState="true"></asp:TextBox>
                                                </div>
                                                <div class="Box5 FloatEsquerdo" style="padding-top: 5px;">
                                                    <asp:ImageButton ID="btnPesquisaProfissionalSubstituto" ToolTip="Pesquisar profissional substituto" OnClick="btnPesquisaProfissionalPrincipal_Click" runat="server" ImageUrl="~/img/lupa.png" CommandName="ProfissionalSubstituto" />
                                                </div>

                                                <div class="BoxFull FloatEsquerdo" style="padding: 5px 0 5px 0;">
                                                    <asp:Label ID="label1" Text="Limite" runat="server" />
                                                </div>
                                                <div class="Box30 FloatEsquerdo">
                                                    <asp:TextBox ID="Limite" onpaste="return false" ondrop="return false" runat="server" Width="95%" Enabled="true" ViewStateMode="Enabled" EnableViewState="true"></asp:TextBox>
                                                </div>
                                                <div class="Box60 FloatEsquerdo" style="padding: 10px 0 0 10px;">
                                                    <asp:Label ID="tpLimite" runat="server" />
                                                </div>
                                                <div class="BoxFull FloatEsquerdo" style="display:flex;">
                                                    <div>
                                                        <asp:CheckBox ID="FL_RECEBE_APRAZ" runat="server" TabIndex="4" Checked="true" CssClass="margin-checkbox"/>
                                                    </div>
                                                    <div >
                                                        <div style="margin-top: 4px;">
                                                            <span style="margin-top: 15px;">Recebe Aprazamento</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </fieldset>

                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnAtualizarGrade" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="LinhaSeparacaoPeq"></div>
                            <div class="Box99 FloatEsquerdo Centro" style="margin: 0px 0px;">
                                <asp:UpdatePanel ID="UpdatePanel3" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:Button ID="btnConfModalProfissional" CssClass="BotaoEsquerdo" ValidationGroup="Modal" runat="server" Text="Confirmar"
                                            ToolTip="Clique para vincular o profissional a equipe" OnClick="btnConfModalProfissional_Click" />
                                        <asp:Button ID="btnCancelModalProfissional" CssClass="BotaoDireitoBranco" runat="server" Text="Voltar"
                                            ToolTip="Clique para voltar para a tela anterior" OnClick="btnCancelModalProfissional_Click" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </fieldset>
                    </asp:Panel>

                    <asp:Button runat="server" ID="btnshowpopupFake" CssClass="Escondido" Width="10px" />
                </div>

                <asp:Button ID="btnAtualizarGrade" runat="server" Text="Atualizar (escondido)" CssClass="Escondido" OnClick="btnAtualizarGrade_Click" />

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

