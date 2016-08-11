using ArdyssLife.PaginaBase;
using Data.Clases;
using Data.Db.Support;
using Data.Db.Tables;
using Data.ExigoApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace ArdyssLife
{
    public partial class WebTransactions : BasePage
    {

        #region Members
        public static BackOfficeV2.Clases.vwPedido _Pedido = new BackOfficeV2.Clases.vwPedido();
        public static List<BackOfficeV2.Clases.vwPedido> _PedidoResponse = null;
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {


        }
        #endregion
        #region ViewCart
        [WebMethod]
        public static List<string> RemoveItemViewCart(string ItemCode)
        {
            try
            {

                if (!CurrentCustomer.TieneValor() || CurrentCustomer.Customer_id == 1)
                {
                    return new List<string>() { "SessionEnd" };
                }

                List<string> Response = new List<string>();

                for (int i = ItemsList.Rows.Count - 1; i >= 0; i--)
                {
                    if (ItemsList.Rows[i].ItemArray[(int)ItemData.ParentItemCode].ToString().Equals(ItemCode) ||
                        ItemsList.Rows[i].ItemArray[(int)ItemData.ItemCode].ToString().Equals(ItemCode))
                    {
                        ItemsList.Rows.RemoveAt(i);
                    }
                }


                if (ItemsList.Rows.Count == 1 && ItemsList.Rows[0].ItemArray[(int)ItemData.ItemCode].ToString().Equals("Z23-1"))
                {
                    RemoveAllViewCart();
                }


                FillDataShipping();

                Response.Add(ItemsList.Rows.Count.ToString());
                return Response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [WebMethod]
        public static string RemoveAllViewCart()
        {
            try
            {
                if (!CurrentCustomer.TieneValor() || CurrentCustomer.Customer_id == 1)
                {
                    return "SessionEnd";
                }

                for (int i = ItemsList.Rows.Count - 1; i >= 0; i--)
                {
                    ItemsList.Rows.RemoveAt(i);
                }

                FillDataShipping();
                return "";
            }
            catch (Exception)
            {

                throw;
            }
        }
        [WebMethod]
        public static List<string> SaveCartShop(string ddlSize, string lbSku, string lbName, string ddlQuantity,
             string lbVp, string lbQp, string lbPrice, string hlpItemLargeImage, string hfPriceTax, string IsAddPromotionItem)
        {

            try
            {
                if (!CurrentCustomer.TieneValor() || CurrentCustomer.Customer_id == 1)
                {
                    return new List<string>() { "SessionEnd" };
                }


                if (IsAddPromotionItem.Equals("true") && CurrentIsAddItemPromotionQP)
                {
                    throw new Exception(CurrentLanguage.Languageid == 1 ? "Solo se puede agregar un producto de tipo promocion" : "You can only add a promotional product type");
                }

                DataRow fila = ItemsList.NewRow();
                List<string> response = new List<string>();

                if (ddlSize.TieneValor())
                {
                    fila[(int)ItemData.ItemCode] = ddlSize;
                }
                else
                {
                    fila[(int)ItemData.ItemCode] = lbSku;
                }

                fila[(int)ItemData.Description] = lbName;
                fila[(int)ItemData.Quantity] = ddlQuantity;

                CultureInfo us = new CultureInfo("en-US");

                fila[(int)ItemData.CommissionableVolumeEach] = lbVp;
                fila[(int)ItemData.BusinessVolumeEach] = lbQp;
                fila[(int)ItemData.PriceEach] = lbPrice;
                fila[(int)ItemData.priceTotal] = Convert.ToDecimal(ddlQuantity) * Convert.ToDecimal(lbPrice);
                fila[(int)ItemData.TaxableEach] = hfPriceTax;
                fila[(int)ItemData.UrlImage] = hlpItemLargeImage;
                fila[(int)ItemData.IsAddPromotionItemQP] = IsAddPromotionItem;

                ItemsList.Rows.Add(fila);
                response.Add(ItemsList.Rows.Count.ToString());
                FillDataShipping();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message);
            }


        }
        [WebMethod]
        public static void CleanItemsShowSearch()
        {
            try
            {
                ItemsToShowBySearch = null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [WebMethod]
        public static List<BackOfficeV2.Clases.ShipMethod> FillShipmethod(Int32 CountryId, Int32 StateId, string CityId)
        {
            try
            {
                if (!CurrentCustomer.TieneValor() || CurrentCustomer.Customer_id == 1)
                {
                    List<BackOfficeV2.Clases.ShipMethod> _ShipMethodResponse = new List<BackOfficeV2.Clases.ShipMethod>();
                    BackOfficeV2.Clases.ShipMethod _ShipMethodModel = new BackOfficeV2.Clases.ShipMethod();

                    _ShipMethodModel.Description = "SessionEnd";
                    _ShipMethodResponse.Add(_ShipMethodModel);

                    return _ShipMethodResponse;
                }


                #region Obtener los datos de Pais,Estado, y Ciudad
                List<BackOfficeV2.Clases.ShipMethod> _ShipMethod = new List<BackOfficeV2.Clases.ShipMethod>();
                BackOfficeV2.Clases.Country _Country = ClmasterData.ClgetCountryById(CountryId);
                if (!_Country.TieneValor())
                {
                    _ShipMethod.Add(new BackOfficeV2.Clases.ShipMethod()
                    {
                        ShipMethodId = 0,
                        Description = "Error en el Pais " + StateId + " " + CountryId

                    });
                }
                List<BackOfficeV2.Clases.State> _State = ClmasterData.ClgetStateByCountryId(CountryId);
                if (!_State.TieneValores())
                {
                    _ShipMethod.Add(new BackOfficeV2.Clases.ShipMethod()
                    {
                        ShipMethodId = 0,
                        Description = "Error en el estado " + StateId + " " + CountryId

                    });
                }
                List<BackOfficeV2.Clases.City> _City = ClmasterData.ClSearchCity(StateId, ClmasterData.ClgetCityById(Convert.ToInt32(CityId)).City_Name);

                #endregion

                #region Modificacion
                if (_City.TieneValores())
                {
                    List<BackOfficeV2.Clases.vwCityShipMethod> _vwCityShipMethod1 = ClmasterData.ClSearchCityShipMethodForReplicated(_City[0].IdCity, false);
                    if (_vwCityShipMethod1.TieneValores())
                    {
                        _vwCityShipMethod1.ForEach(p =>
                        {
                            _ShipMethod.Add(new BackOfficeV2.Clases.ShipMethod()
                            {
                                ShipMethodId = p.ShipmethodId,
                                Description = p.ShipmethodName,
                                IdWarehouse = p.IdWarehouse,
                                IsForBo = p.IsForBo,
                                IsForPickup = p.IsForPickup,
                                IdCarrier = p.IdCarrier,
                                IsActive = p.IsActive,
                                IsForRo = p.IsForRo,
                                IsForEnroll = p.IsForEnroll

                            });
                        });

                    }
                }
                if (!_ShipMethod.TieneValores()) // Cuando la ciudad no tiene valores y bisamos por el estado 
                {
                    List<BackOfficeV2.Clases.vwStateShipMethod> _vwStateShipMethod1 = ClmasterData.ClSearchShipMethodByCountryState(_Country.Id, _State[0].Id);
                    if (_vwStateShipMethod1.TieneValores())
                    {
                        _vwStateShipMethod1.ForEach(p =>
                        {
                            _ShipMethod.Add(new BackOfficeV2.Clases.ShipMethod()
                            {
                                ShipMethodId = p.IdShipmethod,
                                Description = p.Description,
                                IdWarehouse = p.IdWarehouse,
                                IsForBo = p.IsForBo,
                                IsForPickup = p.IsForPickup,
                                IdCarrier = p.IdCarrier,
                                IsActive = p.IsActive,
                                IsForRo = p.IsForRo,
                                IsForEnroll = p.IsForEnroll

                            });
                        });
                    }

                }
                if (!_ShipMethod.TieneValores())
                {
                    List<BackOfficeV2.Clases.vwWarehouseShip> _vwWarehouseShip = ClmasterData.ClSearchShipMethodByCountry(_Country.Id, CurrentWarehouse.WarehouseId);
                    if (_vwWarehouseShip.TieneValores())
                    {
                        _vwWarehouseShip.ForEach(p =>
                        {
                            _ShipMethod.Add(new BackOfficeV2.Clases.ShipMethod()
                            {
                                ShipMethodId = p.shipMethodId,
                                Description = p.ShipMethodName,
                                IdWarehouse = p.warehouseid,
                                IsForBo = p.IsForBO,
                                IsForPickup = p.IsForPickup,
                                IsActive = p.IsActive,
                                IsForRo = p.IsForRo,
                                IsForEnroll = p.IsForEnroll
                            });
                        });
                    }
                }
                return _ShipMethod;
                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }


        [WebMethod]
        public static string GetShippingDiscountOrder()
        {
            try
            {

                if (!CurrentCustomer.TieneValor() || CurrentCustomer.Customer_id == 1)
                {
                    return "SessionEnd";
                }

                if (CurrentShippingDiscountWCTP.TieneValor() && CurrentShippingDiscountWCTP > 0)
                {
                    return string.Format("{0:N2}", CurrentShippingDiscountWCTP);
                }
                else
                {
                    return string.Format("{0:N2}", 0);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region MasterData
        [WebMethod]
        public static List<BackOfficeV2.Clases.City> getCities(string IdState)
        {

            try
            {

                if (!CurrentCustomer.TieneValor() || CurrentCustomer.Customer_id == 1)
                {
                    List<BackOfficeV2.Clases.City> _CityResponse = new List<BackOfficeV2.Clases.City>();
                    BackOfficeV2.Clases.City _CityModel = new BackOfficeV2.Clases.City();

                    _CityModel.City_id = "SessionEnd";
                    _CityResponse.Add(_CityModel);

                    return _CityResponse;
                }

                List<BackOfficeV2.Clases.City> lista_States = Data.Clases.ClmasterData.ClSearchCity(Convert.ToInt32(IdState), "");
                return lista_States;
            }
            catch (Exception)
            {

                throw;
            }


        }

        [WebMethod]
        public static void setCurrentLanguage(string selectedLanguageId)
        {
            BasePage.SetCurrentLanguage(Convert.ToInt32(selectedLanguageId));
        }
        #endregion
        #region Calculater
        [WebMethod]
        public static List<BackOfficeV2.Clases.vwPedido> CalculaOrderByExigo(Int32 ShipmethodId, Int32 StateId, string CityId, string ZipCode, Int32 ApplyDiscount)
        {
            List<BackOfficeV2.Clases.vwPedido> _vwPedidoResponse = new List<BackOfficeV2.Clases.vwPedido>();
            BackOfficeV2.Clases.vwPedido _vwPedidoResponseModel = new BackOfficeV2.Clases.vwPedido();

            try
            {
                decimal Handling = 0;

                if (ItemsList.Rows.Count > 0)
                {
                    if (ApplyDiscount == 1)
                    {
                        for (Int32 i = 0; i < ItemsList.Rows.Count; i++)
                        {
                            if (ItemsList.Rows[i].ItemArray[(int)ItemData.ItemCode].ToString() == "SHDIS" || ItemsList.Rows[i].ItemArray[(int)ItemData.ItemCode].ToString() == "Z23-1")
                            {
                                ItemsList.Rows[i].Delete();
                            }
                        }
                    }
                    else
                    {
                        for (Int32 i = 0; i < ItemsList.Rows.Count; i++)
                        {
                            if (ItemsList.Rows[i].ItemArray[(int)ItemData.ItemCode].ToString() == "Z23-1")
                            {
                                ItemsList.Rows[i].Delete();
                            }
                        }
                    }


                    BackOfficeV2.Clases.State _State = ClmasterData.ClgetStateById(StateId);
                    var lstOrderDetails = new List<Data.ExigoApi.OrderDetailRequest>();

                    _PedidoSession = new BackOfficeV2.Clases.vwPedido();
                    _PedidoSession.Pedido_l = new List<BackOfficeV2.Clases.Pedido_l>();

                    for (Int32 i = 0; i < ItemsList.Rows.Count; i++)
                    {
                        CultureInfo us = new CultureInfo("en-US");
                        var OrderDetails = new Data.ExigoApi.OrderDetailRequest();
                        OrderDetails.ItemCode = ItemsList.Rows[i].ItemArray[(int)ItemData.ItemCode].ToString();
                        OrderDetails.Quantity = Convert.ToDecimal(ItemsList.Rows[i].ItemArray[(int)ItemData.Quantity]);
                        OrderDetails.ParentItemCode = ItemsList.Rows[i].ItemArray[(int)ItemData.ParentItemCode].ToString();
                        //OrderDetails.PriceEachOverride = Decimal.Parse(ItemsList.Rows[i].ItemArray[(int)ItemData.PriceEach].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        // OrderDetails.TaxableEachOverride = Decimal.Parse(ItemsList.Rows[i].ItemArray[(int)ItemData.TaxableEach].ToString().Equals("")?"0":ItemsList.Rows[i].ItemArray[(int)ItemData.TaxableEach].ToString(), System.Globalization.CultureInfo.InvariantCulture); 
                        lstOrderDetails.Add(OrderDetails);

                        BackOfficeV2.Clases.Pedido_l _Pedido_l = new BackOfficeV2.Clases.Pedido_l();
                        _Pedido_l.Item_code = ItemsList.Rows[i].ItemArray[(int)ItemData.ItemCode].ToString();
                        _Pedido_l.Item_name = ItemsList.Rows[i].ItemArray[(int)ItemData.Description].ToString();
                        _Pedido_l.Quantity = Convert.ToDecimal(ItemsList.Rows[i].ItemArray[(int)ItemData.Quantity].ToString());
                        _Pedido_l.CommissionableVolume_each = Decimal.Parse(ItemsList.Rows[i].ItemArray[(int)ItemData.CommissionableVolumeEach].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        _Pedido_l.BusinessVolume_each = Decimal.Parse(ItemsList.Rows[i].ItemArray[(int)ItemData.BusinessVolumeEach].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        _Pedido_l.Commissionable_volume = Decimal.Parse(ItemsList.Rows[i].ItemArray[(int)ItemData.CommissionableVolumeEach].ToString(), System.Globalization.CultureInfo.InvariantCulture) * Convert.ToDecimal(ItemsList.Rows[i].ItemArray[(int)ItemData.Quantity]);
                        _Pedido_l.BusinesVolume = Decimal.Parse(ItemsList.Rows[i].ItemArray[(int)ItemData.BusinessVolumeEach].ToString(), System.Globalization.CultureInfo.InvariantCulture) * Convert.ToDecimal(ItemsList.Rows[i].ItemArray[(int)ItemData.Quantity]);
                        _Pedido_l.Price_each = Decimal.Parse(ItemsList.Rows[i].ItemArray[(int)ItemData.PriceEach].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        _Pedido_l.Price_total = _Pedido_l.Price_each * _Pedido_l.Quantity;

                        //Decimal.Parse(itemToAdd[(int)BasePage.ItemData.PriceEach].ToString(), System.Globalization.CultureInfo.InvariantCulture) * Convert.ToDecimal(itemToAdd[(int)BasePage.ItemData.Quantity]);

                        _Pedido_l.ParentItem_code = ItemsList.Rows[i].ItemArray[(int)ItemData.ParentItemCode].ToString().TieneValor() ? ItemsList.Rows[i].ItemArray[(int)ItemData.ParentItemCode].ToString() : "";
                        _Pedido_l.IdWarehouse = CurrentWarehouse.WarehouseId;
                        _Pedido_l.TaxableEach = Decimal.Parse(ItemsList.Rows[i].ItemArray[(int)ItemData.TaxableEach].ToString(), System.Globalization.CultureInfo.InvariantCulture)/* Decimal.Parse(OrderDetails.TaxableEachOverride.ToString(), System.Globalization.CultureInfo.InvariantCulture)*/;
                        _Pedido_l.TaxableTotal = _Pedido_l.TaxableEach * _Pedido_l.Quantity;
                        _Pedido_l.Taxable = _Pedido_l.TaxableEach * _Pedido_l.Quantity;

                        _PedidoSession.Pedido_l.Add(_Pedido_l);
                    }

                    var CalcOrderReq = new Data.ExigoApi.CalculateOrderRequest();

                    CalcOrderReq.City = CityId.TieneValor() ? CityId.ToString() : "0";
                    CalcOrderReq.Country = CurrentCountry.CountryCode;
                    CalcOrderReq.County = String.Empty; // this value is filled only for US
                    CalcOrderReq.CurrencyCode = CurrentCurrency.CurrencyCode;
                    CalcOrderReq.CustomerID = CurrentCustomer.Customer_id;
                    CalcOrderReq.Details = lstOrderDetails.ToArray();
                    CalcOrderReq.PriceType = CurrentPriceType.PriceTypeId; // TODO:Quitar el valor Fijo y cambiarlo por el tio de cliente correspondiente
                    CalcOrderReq.ShipMethodID = ShipmethodId; // TODO: Cambiar el valor fijo del ShipMethod;
                    CalcOrderReq.State = _State.StateCode;
                    CalcOrderReq.WarehouseID = CurrentWarehouse.WarehouseId;
                    CalcOrderReq.Zip = ZipCode.Equals("") || CurrentCountry.Id == 5 ? "00000" : ZipCode; // this value is filled only for US


                    CalculateOrderResponse CalcOrderRes = Exigo.CalculateOrderRes(CalcOrderReq);
                    int idx = 0;

                    #region llenar el encabezado
                    _PedidoSession.IdPedidoExigo = 0;
                    _PedidoSession.IdWarehouse = CurrentWarehouse.WarehouseId;
                    _PedidoSession.CustomerId = CurrentCustomer.Customer_id;
                    _PedidoSession.CustomerName = CurrentCustomer.First_name + " " + CurrentCustomer.Last_name;
                    _PedidoSession.StateCode = _State.TieneValor() ? _State.StateCode : "";
                    _PedidoSession.IdCurrency = CurrentCurrency.Id;
                    _PedidoSession.IdShipMethod = ShipmethodId;
                    _PedidoSession.OrderCreated = DateTime.Now;
                    _PedidoSession.OrderDate = DateTime.Now;
                    _PedidoSession.CreatedBy = CurrentApiAuthentication.LoginName;
                    _PedidoSession.Address1 = CurrentCustomer.Main_address1;//Direccion al que se va enviar
                    _PedidoSession.Address2 = CurrentCustomer.Main_address2; //Direccion al que se va enviar
                    _PedidoSession.City_id = CityId.TieneValor() ? CityId.ToString() : "0";//Direccion al que se va enviar
                    _PedidoSession.IdState = StateId;
                    _PedidoSession.IdCountry = CurrentCountry.Id;
                    _PedidoSession.IdFiscal = CurrentCustomer.Tax_id.TieneValor() ? CurrentCustomer.Tax_id : "";
                    _PedidoSession.CountryCode = CurrentCountry.CountryCode;
                    _PedidoSession.IdPaymentType = 8;
                    _PedidoSession.FirstName = CurrentCustomer.First_name;//Direccion al que se va enviar
                    _PedidoSession.LastName = CurrentCustomer.Last_name;//Direccion al que se va enviar
                    _PedidoSession.ZipCode = CalcOrderReq.Zip;//Direccion al que se va enviar
                    _PedidoSession.OrderStatusId = CurrentWarehouse.WarehouseId == 1 || CurrentWarehouse.WarehouseId == 127 ? (int)OrderStatusType.Pending : (int)OrderStatusType.Accepted;

                    if (CurrentWarehouse.TieneValor() && CurrentWarehouse.IdForAccounting.NotieneBlancos())
                    {
                        _PedidoSession.OrderDate = CurrentWarehouse.IdForAccounting.ConvertFecha();
                    }

                    _PedidoSession.IdOrderType = 2; //TODO POner el Customer Type Correcto
                    _PedidoSession.OrderType = "Customer Service";
                    _PedidoSession.OrderTypeName = "Customer Service";
                    _PedidoSession.Phone = CurrentCustomer.Phone;//Direccion al que se va enviar
                    _PedidoSession.IsActive = true;
                    _PedidoSession.IdPriceType = CurrentPriceType.PriceTypeId;
                    _PedidoSession.paymentTypeName = "Cash";
                    _PedidoSession.IdStatus = CurrentWarehouse.WarehouseId == 1 || CurrentWarehouse.WarehouseId == 127 ? (int)OrderStatusType.Pending : (int)OrderStatusType.Accepted;
                    _PedidoSession.SubtotalOrder = CalcOrderRes.SubTotal;
                    _PedidoSession.ShipAmount = CalcOrderRes.ShippingTotal;
                    _PedidoSession.TotalTax = CalcOrderRes.TaxTotal;
                    _PedidoSession.TotalOrder = CalcOrderRes.Total;
                    _PedidoSession.VP = CalcOrderRes.CommissionableVolumeTotal;
                    _PedidoSession.QP = CalcOrderRes.BusinessVolumeTotal;
                    _PedidoSession.TotalTax = CalcOrderRes.TaxTotal;
                    #endregion
                    #region Llenar Detalle
                    _PedidoSession.Pedido_l.Clear();


                    foreach (OrderDetailResponse orderDetail in CalcOrderRes.Details)
                    {

                        BackOfficeV2.Clases.Pedido_l _Pedido_l = new BackOfficeV2.Clases.Pedido_l();
                        _Pedido_l.Item_code = orderDetail.ItemCode;
                        _Pedido_l.Item_name = orderDetail.Description;
                        _Pedido_l.Quantity = orderDetail.Quantity;
                        _Pedido_l.BusinessVolume_each = orderDetail.BusinessVolumeEach;
                        _Pedido_l.CommissionableVolume_each = orderDetail.CommissionableVolume;
                        _Pedido_l.Commissionable_volume = orderDetail.CommissionableVolume;
                        _Pedido_l.BusinesVolume = orderDetail.BusinesVolume;
                        _Pedido_l.Price_each = orderDetail.PriceEach;
                        _Pedido_l.Price_total = orderDetail.PriceTotal;
                        _Pedido_l.ParentItem_code = orderDetail.ParentItemCode;
                        _Pedido_l.IdWarehouse = CurrentWarehouse.WarehouseId;
                        _Pedido_l.TaxableEach = orderDetail.Taxable;
                        _Pedido_l.TaxableTotal = orderDetail.Taxable;
                        _Pedido_l.Taxable = orderDetail.Taxable;
                        _Pedido_l.Tax = orderDetail.Tax;

                        _PedidoSession.Pedido_l.Add(_Pedido_l);


                        if (orderDetail.ItemCode != "Z23-1")
                        {
                            // Subtotal += orderDetail.PriceTotal;
                            bool ban = false;
                            idx = 0;

                            _PedidoSession.VP = 0;
                            _PedidoSession.QP = 0;

                            foreach (DataRow dr in ItemsList.Rows)
                            {

                                _PedidoSession.VP += Convert.ToDecimal(dr.ItemArray[(int)ItemData.CommissionableVolumeEach]);
                                _PedidoSession.QP += Convert.ToDecimal(dr.ItemArray[(int)ItemData.BusinessVolumeEach]);


                                if (orderDetail.ParentItemCode.TieneValor())
                                {
                                    if (dr.ItemArray[(int)ItemData.ItemCode].Equals(orderDetail.ItemCode) && dr.ItemArray[(int)ItemData.ParentItemCode].Equals(orderDetail.ParentItemCode))
                                    {
                                        ban = true;
                                    }
                                }
                                else
                                {
                                    if (dr.ItemArray[(int)ItemData.ItemCode].Equals(orderDetail.ItemCode))
                                    {
                                        ban = true;
                                    }
                                }

                                //si existe el codigo entonces se sale
                                if (ban)
                                {
                                    break;
                                }
                                idx++;

                            }

                            if (ban)
                            {
                                //actualiza los valores de los items que ya existen
                                ItemsList.Rows[idx].ItemArray[(int)ItemData.BusinessVolumeEach] = string.Format("{0:N}", orderDetail.BusinesVolume);
                                ItemsList.Rows[idx].ItemArray[(int)ItemData.CommissionableVolumeEach] = string.Format("{0:N}", orderDetail.CommissionableVolume);
                                ItemsList.Rows[idx].ItemArray[(int)ItemData.PriceEach] = string.Format("{0:N}", orderDetail.PriceEach);
                                ItemsList.Rows[idx].ItemArray[(int)ItemData.priceTotal] = string.Format("{0:N}", orderDetail.PriceTotal);

                            }
                            else
                            {
                                //inserta los items que no existen
                                DataRow fila = ItemsList.NewRow();
                                fila[(int)ItemData.ItemCode] = orderDetail.ItemCode;
                                fila[(int)ItemData.Description] = orderDetail.Description;
                                fila[(int)ItemData.Quantity] = string.Format("{0:N}", orderDetail.Quantity);
                                fila[(int)ItemData.BusinessVolumeEach] = string.Format("{0:N}", orderDetail.BusinesVolume);
                                fila[(int)ItemData.CommissionableVolumeEach] = string.Format("{0:N}", orderDetail.CommissionableVolume);
                                fila[(int)ItemData.PriceEach] = string.Format("{0:N}", orderDetail.PriceEach);
                                fila[(int)ItemData.priceTotal] = string.Format("{0:N}", orderDetail.PriceTotal);
                                fila[(int)ItemData.ParentItemCode] = orderDetail.ParentItemCode;

                                if (orderDetail.ParentItemCode.TieneValor())
                                {
                                    // fila[(int)ItemData.ParentItemCode] = String.Format("{0}@{1}", ItemsList.Rows.Count, orderDetail.ParentItemCode); 
                                }

                                ItemsList.Rows.Add(fila);
                            }


                        }
                        else
                        {
                            Handling = orderDetail.PriceTotal;
                        }


                    }
                    #endregion

                    CurrentShippingDiscountWCTP = ClPromotions.ClGetPromotionsShippingDiscountWCTP(_PedidoSession, CurrentCustomer.Customer_type, CurrentWarehouse.WarehouseId, CurrentCountry.Id);

                }
                _vwPedidoResponse.Add(_PedidoSession);
                int number4 = 4;
                DataItemShipping _DataItemShipping = new DataItemShipping();
                _DataItemShipping.IdState = StateId;
                _DataItemShipping.ZipCode = _PedidoSession.ZipCode;
                _DataItemShipping.SubTotalOrder = string.Format("{0:N2}", _vwPedidoResponse[0].SubtotalOrder);
                _DataItemShipping.TotalQP = string.Format("{0:N2}", _vwPedidoResponse[0].QP);
                _DataItemShipping.TotalVP = string.Format("{0:N2}", _vwPedidoResponse[0].VP);
                _DataItemShipping.CityName = CityId.TieneValor() ? CityId : (CurrentDataItemShipping.TieneValor() ? CurrentDataItemShipping.CityName : "");
                _DataItemShipping.IdCity = int.TryParse(CityId, out number4) ? Convert.ToInt32(CityId) : 0;
                _DataItemShipping.IdShipMethod = ShipmethodId;
                _DataItemShipping.Tax = string.Format("{0:N2}", _vwPedidoResponse[0].TotalTax);
                _DataItemShipping.Handling = string.Format("{0:N2}", Handling);
                _DataItemShipping.TotalOrder = string.Format("{0:N2}", _vwPedidoResponse[0].TotalOrder);

                CurrentDataItemShipping = _DataItemShipping;

                return _vwPedidoResponse;
            }
            catch (Exception)
            {
                return _vwPedidoResponse;
            }
        }

        [WebMethod]
        public static List<BackOfficeV2.Clases.vwPedido> CalculaOrder(Int32 ShipmethodId, Int32 StateId, Int32 CityId, string ZipCode, Int32 ApplyDiscount)
        {
            try
            {

                if (!CurrentCustomer.TieneValor() || CurrentCustomer.Customer_id == 1)
                {
                    List<BackOfficeV2.Clases.vwPedido> _vwPedidoResponse2 = new List<BackOfficeV2.Clases.vwPedido>();
                    BackOfficeV2.Clases.vwPedido _vwPedidoModel = new BackOfficeV2.Clases.vwPedido();

                    _vwPedidoModel.CustomerName = "SessionEnd";
                    _vwPedidoResponse2.Add(_vwPedidoModel);

                    return _vwPedidoResponse2;
                }


                decimal BaseParaDescuento = 0;
                decimal Discount = 0;
                BackOfficeV2.Clases.State _State = ClmasterData.ClgetStateById(StateId);

                if (ItemsList.Rows.Count > 0 && ApplyDiscount == 1)
                {

                    for (Int32 i = 0; i < ItemsList.Rows.Count; i++)
                    {
                        if (ItemsList.Rows[i].ItemArray[(int)ItemData.ItemCode].ToString() == "SHDIS")
                        {
                            ItemsList.Rows[i].Delete();
                        }
                    }

                    #region llenar el encabezado
                    _Pedido.IdPedidoExigo = 0;
                    _Pedido.IdWarehouse = CurrentWarehouse.WarehouseId;
                    _Pedido.CustomerId = CurrentCustomer.Customer_id;
                    _Pedido.CustomerName = CurrentCustomer.First_name + " " + CurrentCustomer.Last_name;
                    _Pedido.StateCode = _State.TieneValor() ? _State.StateCode : "";
                    _Pedido.IdCurrency = CurrentCurrency.Id;
                    _Pedido.IdShipMethod = ShipmethodId;
                    _Pedido.ShipAmount = 0;
                    _Pedido.OrderCreated = DateTime.Now;
                    _Pedido.OrderDate = DateTime.Now;
                    _Pedido.CreatedBy = CurrentApiAuthentication.LoginName;
                    _Pedido.Address1 = CurrentCustomer.Main_address1;//Direccion al que se va enviar
                    _Pedido.Address2 = CurrentCustomer.Main_address2; //Direccion al que se va enviar
                    _Pedido.City_id = CityId.ToString();//Direccion al que se va enviar
                    _Pedido.IdState = StateId;
                    _Pedido.IdCountry = CurrentCountry.Id;
                    _Pedido.CountryCode = CurrentCountry.CountryCode;
                    _Pedido.IdPaymentType = 8;
                    _Pedido.IdFiscal = CurrentCustomer.Tax_id.TieneValor() ? CurrentCustomer.Tax_id : "";
                    _Pedido.FirstName = CurrentCustomer.First_name;//Direccion al que se va enviar
                    _Pedido.LastName = CurrentCustomer.Last_name;//Direccion al que se va enviar
                    _Pedido.ZipCode = ZipCode;//Direccion al que se va enviar
                    _Pedido.OrderStatusId = (int)OrderStatusType.Pending;

                    if (CurrentWarehouse.TieneValor() && CurrentWarehouse.IdForAccounting.NotieneBlancos())
                    {
                        _Pedido.OrderDate = CurrentWarehouse.IdForAccounting.ConvertFecha();
                    }

                    _Pedido.TotalTax = 0; // esto es para llenar el impuesto de la orden cuando se guarda
                    _Pedido.IdOrderType = 2; //TODO POner el Customer Type Correcto
                    _Pedido.OrderType = "Customer Service";
                    _Pedido.OrderTypeName = "Customer Service";
                    _Pedido.Phone = CurrentCustomer.Phone;//Direccion al que se va enviar
                    _Pedido.IsActive = true;
                    _Pedido.IdPriceType = CurrentPriceType.PriceTypeId;
                    _Pedido.paymentTypeName = "Cash";
                    _Pedido.IdStatus = 7; // TODO Poner el estatus correcto 
                    #endregion
                    #region Llenar las Lineas del pedido
                    for (Int32 i = 0; i < ItemsList.Rows.Count; i++)
                    {
                        if (ItemsList.Rows[i].ItemArray[0].ToString() == "Z23-1")
                        {
                            ItemsList.Rows[i].Delete();
                        }
                    }
                    _Pedido.Pedido_l = new List<BackOfficeV2.Clases.Pedido_l>();
                    foreach (DataRow dr in ItemsList.Rows)
                    {

                        BackOfficeV2.Clases.Pedido_l _Pedido_l = new BackOfficeV2.Clases.Pedido_l();
                        _Pedido_l.Item_code = dr.ItemArray[(int)ItemData.ItemCode].ToString();
                        _Pedido_l.Item_name = dr.ItemArray[(int)ItemData.Description].ToString();
                        _Pedido_l.Quantity = Convert.ToDecimal(dr.ItemArray[(int)ItemData.Quantity].ToString());
                        _Pedido_l.CommissionableVolume_each = Convert.ToDecimal(dr.ItemArray[(int)ItemData.CommissionableVolumeEach]);
                        _Pedido_l.BusinessVolume_each = Convert.ToDecimal(dr.ItemArray[(int)ItemData.BusinessVolumeEach]);
                        _Pedido_l.Commissionable_volume = Convert.ToDecimal(dr.ItemArray[(int)ItemData.CommissionableVolumeEach]) * Convert.ToDecimal(dr.ItemArray[(int)ItemData.Quantity]);
                        _Pedido_l.BusinesVolume = Convert.ToDecimal(dr.ItemArray[(int)ItemData.BusinessVolumeEach]) * Convert.ToDecimal(dr.ItemArray[(int)ItemData.Quantity]);
                        _Pedido_l.Price_each = Convert.ToDecimal(dr.ItemArray[(int)ItemData.PriceEach]);
                        _Pedido_l.Price_total = Convert.ToDecimal(dr.ItemArray[(int)ItemData.priceTotal]);
                        _Pedido_l.ParentItem_code = dr.ItemArray[(int)ItemData.TaxableEach].ToString().TieneValor() ? dr.ItemArray[(int)ItemData.TaxableEach].ToString() : "";
                        _Pedido_l.IdWarehouse = CurrentWarehouse.WarehouseId;
                        _Pedido_l.TaxableEach = Convert.ToDecimal(dr.ItemArray[(int)ItemData.TaxableEach].TieneValor() ? dr.ItemArray[(int)ItemData.TaxableEach] : 0);
                        _Pedido_l.TaxableTotal = Convert.ToDecimal(dr.ItemArray[(int)ItemData.TaxableEach].TieneValor() ? dr.ItemArray[(int)ItemData.TaxableEach] : 0) * Convert.ToDecimal(dr.ItemArray[(int)ItemData.Quantity].TieneValor() ? dr.ItemArray[(int)ItemData.Quantity] : 0);
                        _Pedido_l.Taxable = Convert.ToDecimal(dr.ItemArray[(int)ItemData.TaxableEach].TieneValor() ? dr.ItemArray[(int)ItemData.TaxableEach] : 0) * Convert.ToDecimal(dr.ItemArray[(int)ItemData.Quantity].TieneValor() ? dr.ItemArray[(int)ItemData.Quantity] : 0);
                        _Pedido.Pedido_l.Add(_Pedido_l);
                    }
                    #endregion
                    // Enviar a calcular
                    BackOfficeV2.Clases.vwPedido _vwPedido = Clpedido.CalculaPedido(_Pedido);

                    #region actualizar en la tabla temporal los valores resultado del calculo
                    int idx = 0;
                    _vwPedido.Pedido_l.ForEach(p =>
                    {
                        bool ban = false;
                        foreach (DataRow dr in ItemsList.Rows)
                        {
                            if (p.ParentItem_code.TieneValor())
                            {
                                if (dr.ItemArray[0].Equals(p.Item_code) && dr.ItemArray[7].Equals(p.ParentItem_code))
                                {
                                    ban = true;
                                }
                            }
                            else
                            {
                                if (dr.ItemArray[0].Equals(p.Item_code))
                                {
                                    ban = true;
                                }
                            }

                            //si existe el codigo entonces se sale
                            if (ban)
                            {
                                break;
                            }

                            //idx++;
                        }

                        if (ban)
                        {
                            //actualiza los valores de los items que ya existen
                            ItemsList.Rows[idx][(int)ItemData.CommissionableVolumeEach] = string.Format("{0:N}", p.CommissionableVolume_each);
                            ItemsList.Rows[idx][(int)ItemData.BusinessVolumeEach] = string.Format("{0:N}", p.BusinessVolume_each);
                            ItemsList.Rows[idx][(int)ItemData.PriceEach] = string.Format("{0:N}", p.Price_each);
                            ItemsList.Rows[idx][(int)ItemData.priceTotal] = string.Format("{0:N}", p.Price_total);
                        }
                        else
                        {
                            //inserta los items que no existen
                            DataRow fila = ItemsList.NewRow();

                            fila[(int)ItemData.ItemCode] = p.Item_code;                                             //dtInvTranl.Columns.Add("Codigo");
                            fila[(int)ItemData.Description] = p.Item_name;                                          //dtInvTranl.Columns.Add("Descripcion");
                            fila[(int)ItemData.Quantity] = string.Format("{0:N}", p.Quantity);                     //dtInvTranl.Columns.Add("Cantidad");
                            fila[(int)ItemData.CommissionableVolumeEach] = string.Format("{0:N}", p.CommissionableVolume_each);                //dtInvTranl.Columns.Add("Vp");
                            fila[(int)ItemData.BusinessVolumeEach] = string.Format("{0:N}", p.BusinessVolume_each);         //dtInvTranl.Columns.Add("Qp");
                            fila[(int)ItemData.PriceEach] = string.Format("{0:N}", p.Price_each);                    //dtInvTranl.Columns.Add("priceEach");
                            fila[(int)ItemData.priceTotal] = string.Format("{0:N}", p.Price_total);                   //dtInvTranl.Columns.Add("priceTotal");
                            fila[(int)ItemData.TaxableEach] = p.ParentItem_code;

                            if (p.ParentItem_code.TieneValor())
                            {
                                fila[(int)ItemData.ParentItemCode] = String.Format("{0}@{1}", ItemsList.Rows.Count, p.ParentItem_code);
                            }

                            ItemsList.Rows.Add(fila);
                        }

                        idx++;
                        // Calcular descuentos 
                        if (CurrentWarehouse.AplicaDescuento > 0)
                        {
                            List<BackOfficeV2.Clases.vwWhseItemDiscount> _vwWhseItemDiscount = ClItem.ClGetWhseItemDiscount(CurrentWarehouse.WarehouseId, p.Item_code);
                            if (!_vwWhseItemDiscount.TieneValores())
                            {
                                BaseParaDescuento += p.Price_total;
                            }
                        }
                    });

                    #endregion
                    _PedidoSession = _vwPedido;

                }
                else
                {
                    throw new Exception("El Pedido Esta vacio");
                }

                List<BackOfficeV2.Clases.vwPedido> _vwPedidoResponse = new List<BackOfficeV2.Clases.vwPedido>();
                CurrentShippingDiscountWCTP = ClPromotions.ClGetPromotionsShippingDiscountWCTP(_PedidoSession, CurrentCustomer.Customer_type, CurrentWarehouse.WarehouseId, CurrentCountry.Id);

                if (ApplyDiscount == 1)
                {
                    if (CurrentShippingDiscountWCTP > 0 && ModuloArdyssLife.Equals("4"))
                    {
                        DataRow fila = ItemsList.NewRow();

                        fila[(int)ItemData.ItemCode] = "SHDIS";
                        fila[(int)ItemData.Description] = "Shipping Discount";
                        fila[(int)ItemData.Description] = -1;
                        fila[(int)ItemData.CommissionableVolumeEach] = 0;
                        fila[(int)ItemData.BusinessVolumeEach] = 0;
                        fila[(int)ItemData.PriceEach] = CurrentShippingDiscountWCTP;
                        fila[(int)ItemData.priceTotal] = CurrentShippingDiscountWCTP;
                        fila[(int)ItemData.TaxableEach] = 0;
                        fila[(int)ItemData.IsAddPromotionItemQP] = "false";
                        ItemsList.Rows.Add(fila);

                        BackOfficeV2.Clases.Pedido_l _Pedido_l = new BackOfficeV2.Clases.Pedido_l();
                        _Pedido_l.Item_code = fila[(int)ItemData.ItemCode].ToString();
                        _Pedido_l.Item_name = fila[(int)ItemData.Description].ToString();
                        _Pedido_l.Quantity = Convert.ToDecimal(fila[(int)ItemData.Quantity].ToString());
                        _Pedido_l.BusinessVolume_each = Convert.ToDecimal(fila[(int)ItemData.BusinessVolumeEach]);
                        _Pedido_l.CommissionableVolume_each = Convert.ToDecimal(fila[(int)ItemData.CommissionableVolumeEach]);
                        _Pedido_l.BusinesVolume = Convert.ToDecimal(fila[(int)ItemData.BusinessVolumeEach]) * Convert.ToDecimal(fila[(int)ItemData.Quantity]);
                        _Pedido_l.Commissionable_volume = Convert.ToDecimal(fila[(int)ItemData.CommissionableVolumeEach]) * Convert.ToDecimal(fila[(int)ItemData.Quantity]);
                        _Pedido_l.Price_each = Convert.ToDecimal(fila[(int)ItemData.PriceEach]);
                        _Pedido_l.Price_total = Convert.ToDecimal(fila[(int)ItemData.priceTotal]);
                        _Pedido_l.IdWarehouse = CurrentWarehouse.WarehouseId;
                        _Pedido_l.TaxableEach = Convert.ToDecimal(fila[(int)ItemData.TaxableEach].TieneValor() ? fila[(int)ItemData.TaxableEach] : 0);
                        _Pedido_l.TaxableTotal = Convert.ToDecimal(fila[(int)ItemData.TaxableEach].TieneValor() ? fila[(int)ItemData.TaxableEach] : 0) * Convert.ToDecimal(fila[(int)ItemData.TaxableEach].TieneValor() ? fila[(int)ItemData.TaxableEach] : 0);
                        _Pedido_l.Taxable = Convert.ToDecimal(fila[(int)ItemData.TaxableEach].TieneValor() ? fila[(int)ItemData.TaxableEach] : 0) * Convert.ToDecimal(fila[(int)ItemData.Quantity].TieneValor() ? fila[(int)ItemData.Quantity] : 0);
                        _PedidoSession.Pedido_l.Add(_Pedido_l);

                    }
                }

                _vwPedidoResponse.Add(_PedidoSession);

                return _vwPedidoResponse;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion
        #region Items
        [WebMethod]
        public static Data.Db.Support.DataItemShipping getDataShipping()
        {
            try
            {
                FillDataShipping();
                return CurrentDataItemShipping;
            }
            catch (Exception)
            {

                throw;
            }
        }
        [WebMethod]
        public static Int32 GetCommandLoadScript()
        {
            try
            {
                return CommandLoadScript;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        [WebMethod]
        public static void SetCommandLoadScript(Int32 Command)
        {
            try
            {
                CommandLoadScript = Command;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static GridView gvpacksKits
        {
            get
            {
                return (GridView)HttpContext.Current.Session["gvpacksKits"];
            }
        }

        [WebMethod]
        public static bool IsSearchItem(string ItemDescription)
        {
            bool IsSearch = false;

            try
            {

                ItemsToShowBySearch = ItemsToShow.Where(w => w.Description.Contains(ItemDescription)).ToList();

                if (ItemsToShowLocal.TieneValores())
                {
                    ItemsToShowLocal.ForEach(p =>
                    {
                        IsSearch = true;
                    });

                }
            }
            catch (Exception)
            {

                throw;
            }


            return IsSearch;
        }

        [WebMethod]
        public static List<ListItem> getKitHijos(string ItemCode, string ItemTexto)
        {

            try
            {
                List<ListItem> items = new List<ListItem>();
                //obtener en qu[e fila se encuentra el control
                string texto = ItemTexto;
                int rowindex = Convert.ToInt32(texto.Substring(texto.IndexOf("idx:") + 5));


                //si el item es groupmaster entonces lleno el dropdown


                // Inicia
                List<BackOfficeV2.Clases.GroupMembers> datacontenido = ClItem.GetItemGroupmembers(ItemCode);
                if (!datacontenido.TieneValores())
                {
                    throw new Exception("No estan los group members para este item " + ItemCode);
                }


                List<string> ItemCodes = datacontenido.Select(p => p.ItemCode).ToList();
                for (int inv = 0; inv < datacontenido.Count; inv++)
                {
                    ItemCodes.Add(datacontenido[inv].ItemCode);
                }

                List<BackOfficeV2.Clases.vwItemAllInfo> _vwItemAllInfo1 = ClItem.GetItemAllInfoAvailable(CurrentCountry.Id,
                  Convert.ToInt32(CurrentWarehouse.InventoryMargin),
                  CurrentWarehouse.WarehouseId,
                  CurrentCurrency.CurrencyCode,
                  CurrentPriceType.PriceTypeId,
                  CurrentLanguage.Languageid
                  , false, getAllInfo: false, itemcodes: ItemCodes);

                if (_vwItemAllInfo1.TieneValores())
                {

                    _vwItemAllInfo1.ForEach(p =>
                    {

                        decimal Apartado = 0; // ClItem.GetProdAparta(Convert.ToInt32(ddWarehouse.SelectedValue), item.ItemCode);
                        items.Add(new ListItem(p.Description + " Disponibilidad: " + (60).ToString(), p.ItemCode));

                    });

                }

                return items;
            }
            catch (Exception)
            {
                throw;
            }

        }

        [WebMethod]
        public static void SavePacks(string ItemId, string Description, string Quantity, string Vp, string Qp, string price, string priceTax, string ItemCode, string ListGridView)
        {
            try
            {

                var o = JObject.Parse(ListGridView);
                List<ItemJSon> vv = null;

                foreach (var x in o)
                {
                    string name = x.Key;
                    JToken value = x.Value;//jarray
                    var jt = JToken.Parse(value.ToString());
                    vv = jt.ToObject<List<ItemJSon>>();
                }


                DataRow fila1 = ItemsList.NewRow();
                #region para el proceso normal de almacenes diferentes a Ecuador
                if (CurrentWarehouse.WarehouseId.ToString() != "173")
                {
                    #region Agregar el item Padre
                    //string kitunico = String.Format("{0}@{1}", gvItem.Rows.Count, ItemId);
                    fila1[(int)ItemData.ItemCode] = ItemCode;                                                        // ItemCode
                    fila1[(int)ItemData.Description] = Description;                                                   // ItemDescription
                    fila1[(int)ItemData.Quantity] = string.Format("{0:N}", Convert.ToDecimal(Quantity));                // Quantity
                    fila1[(int)ItemData.CommissionableVolumeEach] = string.Format("{0:N}", Convert.ToDecimal(Vp));              // Vp
                    fila1[(int)ItemData.BusinessVolumeEach] = string.Format("{0:N}", Convert.ToDecimal(Qp));              // Qp
                    fila1[(int)ItemData.PriceEach] = string.Format("{0:N}", Convert.ToDecimal(price));       // PriceEach
                    fila1[(int)ItemData.priceTotal] = string.Format("{0:N}", Convert.ToDecimal(Quantity) * Convert.ToDecimal(price)); // PriceTotal
                    fila1[(int)ItemData.ParentItemCode] = "";                                                                   // ItemParent
                    //fila1[8] = kitunico;
                    fila1[(int)ItemData.TaxableEach] = string.Format("{0:N}", Convert.ToDecimal(priceTax));     // Taxabe Each
                    fila1[(int)ItemData.IsAddPromotionItemQP] = "false";
                    ItemsList.Rows.Add(fila1);
                    #endregion
                    #region Agregar productos Hijos

                    vv.ForEach(p =>
                    {

                        if (p.ItemValue != "0" && Convert.ToBoolean(p.CheckItem) == false)
                        {
                            string description = p.ItemText.Substring(0, p.ItemText.ToUpper().IndexOf("DISPONIBILIDAD")).Trim();
                            DataRow fila = ItemsList.NewRow();
                            fila[(int)ItemData.ItemCode] = p.ItemValue;                                    // ItemCode
                            fila[(int)ItemData.Description] = description;                                                // ItemDescription
                            fila[(int)ItemData.Quantity] = string.Format("{0:N}", Convert.ToDecimal(Quantity));      // Quantity
                            fila[(int)ItemData.CommissionableVolumeEach] = "0";                                                        // Vp
                            fila[(int)ItemData.BusinessVolumeEach] = "0";                                                        // Qp
                            fila[(int)ItemData.PriceEach] = "0";                                                        // Price Each;
                            fila[(int)ItemData.priceTotal] = "0";                                                        // Price Total
                            fila[(int)ItemData.ParentItemCode] = ItemId;                                              // ParentitemCode
                            //fila[8] = kitunico;
                            fila[(int)ItemData.TaxableEach] = "0";
                            fila[(int)ItemData.IsAddPromotionItemQP] = "false";
                            ItemsList.Rows.Add(fila);
                        }
                        else
                        {

                            if (p.ItemSizeValue != "0" && p.ItemValue != "0")
                            {
                                string description = p.ItemText.Substring(0, p.ItemText.ToUpper().IndexOf("DISPONIBILIDAD")).Trim();
                                DataRow fila = ItemsList.NewRow();
                                fila[(int)ItemData.ItemCode] = p.ItemSizeValue;                                           // ItemCode 
                                fila[(int)ItemData.Description] = description;                                                  // Description
                                fila[(int)ItemData.Quantity] = string.Format("{0:N}", Convert.ToDecimal(Quantity));        // Quantity
                                fila[(int)ItemData.CommissionableVolumeEach] = "0";                                                          // Vp
                                fila[(int)ItemData.BusinessVolumeEach] = "0";                                                          // Qp
                                fila[(int)ItemData.PriceEach] = "0";                                                          // PriceEach
                                fila[(int)ItemData.priceTotal] = "0";                                                          // PriceTotal
                                fila[(int)ItemData.ParentItemCode] = ItemId;                                                // ParentitemCode
                                //fila[8] = kitunico;
                                fila[(int)ItemData.TaxableEach] = "0";
                                fila[(int)ItemData.IsAddPromotionItemQP] = "false";
                                ItemsList.Rows.Add(fila);
                            }
                        }
                    });
                    #endregion
                }
                #endregion
                // Manejar los productos hijos cuando se trata del almacen de Ecuador ya que no puede tener valores en 0
                else
                {

                    decimal TotalItems = 0;
                    #region Calcular el numero de Items

                    vv.ForEach(p2 =>
                    {

                        List<BackOfficeV2.Clases.vwItemAllInfo> _vwItemSelected = ClItem.GetItSelectedItem(0, CurrentCountry.Id, p2.ItemValue);
                        if (_vwItemSelected.TieneValores())
                        {
                            List<BackOfficeV2.Clases.ItemPrice> _ItemPrice = ClItem.ClSearchItemPriceByItemIdCurrencyIdPriceTypeId(_vwItemSelected[0].Itemid, Convert.ToInt32(CurrentCurrency.Id), Convert.ToInt32(CurrentPriceType.PriceTypeId));
                            if (_ItemPrice.TieneValores())
                                TotalItems += _ItemPrice[0].Price;
                        }

                    });


                    decimal PrecioPorc = ((Convert.ToDecimal(price) * 100) / TotalItems) / 100;
                    #endregion
                    #region agregar productos como simples

                    vv.ForEach(p3 =>
                    {
                        if (p3.ItemValue != "0" && Convert.ToBoolean(p3.CheckItem) == false)
                        {
                            #region cuando no se trata de tallas
                            string description = p3.ItemText.Substring(0, p3.ItemText.ToUpper().IndexOf("DISPONIBILIDAD")).Trim();
                            List<String> ItemCodes = new List<String>();
                            ItemCodes.Add(p3.ItemValue);
                            List<BackOfficeV2.Clases.vwItemAllInfo> _vwItemAllInfo1 = ClItem.GetItemAllInfoAvailable(CurrentCountry.Id,
                              Convert.ToInt32(CurrentWarehouse.InventoryMargin),
                              CurrentWarehouse.WarehouseId,
                              CurrentCurrency.CurrencyCode,
                              CurrentPriceType.PriceTypeId,
                              CurrentLanguage.Languageid
                              , false, getAllInfo: false, itemcodes: ItemCodes);

                            DataRow fila = ItemsList.NewRow();
                            fila[(int)ItemData.ItemCode] = p3.ItemValue;//txtItemCode.Text;                                                                                      
                            fila[(int)ItemData.Description] = description; //TxtItemName.Text;                                                                          //dtInvTranl.Columns.Add("Descripcion");
                            fila[(int)ItemData.Quantity] = string.Format("{0:N}", Convert.ToDecimal(Quantity));                                                    //dtInvTranl.Columns.Add("Cantidad");

                            fila[(int)ItemData.CommissionableVolumeEach] = string.Format("{0:N}", Convert.ToDecimal(Vp));                                                     //dtInvTranl.Columns.Add("Vp");
                            fila[(int)ItemData.BusinessVolumeEach] = string.Format("{0:N}", Convert.ToDecimal(Qp));                                                     //dtInvTranl.Columns.Add("Qp");
                            fila[(int)ItemData.PriceEach] = string.Format("{0:N}", _vwItemAllInfo1[0].ItemPrice.Price);                                                               //dtInvTranl.Columns.Add("priceEach");
                            //fila[15] = string.Format("{0:N}", _vwItemAllInfo1[0].ItemPrice.Price);                                                              //TaxableType Each
                            fila[(int)ItemData.priceTotal] = string.Format("{0:N}", Convert.ToDecimal(Quantity) * Convert.ToDecimal(Quantity) / vv.Count);       //dtInvTranl.Columns.Add("priceTotal");
                            //fila[9] = "1"; // Modicicaion para ver si aplicamos el descuento 
                            //fila[10] = string.Format("{0:N}", _vwItemAllInfo1[0].ItemPrice.Price * PrecioPorc);                                      // Override price
                            //fila[11] = string.Format("{0:N}", Convert.ToDecimal(Quantity) * _vwItemAllInfo1[0].ItemPrice.Price * PrecioPorc);
                            fila[(int)ItemData.TaxableEach] = string.Format("{0:N}", (_vwItemAllInfo1[0].ItemPrice.Price * Convert.ToDecimal(Quantity)) - _vwItemAllInfo1[0].ItemPrice.Price * PrecioPorc * Convert.ToDecimal(Quantity));
                            fila[(int)ItemData.IsAddPromotionItemQP] = "false";
                            fila[(int)ItemData.BusinessVolumeEach] = string.Format("{0:N}", Convert.ToDecimal(Vp) / vv.Count);                                           //dtInvTranl.Columns.Add("Qp");
                            fila[(int)ItemData.CommissionableVolumeEach] = string.Format("{0:N}", Convert.ToDecimal(Qp) / vv.Count);
                            ItemsList.Rows.Add(fila);

                            #endregion
                        }
                        else
                        {
                            #region cuando se trata de tallas

                            if (p3.ItemSizeValue != "0" && p3.ItemValue != "0")
                            {
                                List<String> ItemCodes = new List<String>();
                                ItemCodes.Add(p3.ItemSizeValue);
                                List<BackOfficeV2.Clases.vwItemAllInfo> _vwItemAllInfo1 = ClItem.GetItemAllInfoAvailable(CurrentCountry.Id,
                             Convert.ToInt32(CurrentWarehouse.InventoryMargin),
                             CurrentWarehouse.WarehouseId,
                             CurrentCurrency.CurrencyCode,
                             CurrentPriceType.PriceTypeId,
                             CurrentLanguage.Languageid
                             , false, getAllInfo: false, itemcodes: ItemCodes);
                                string description = p3.ItemText.Substring(0, p3.ItemText.ToUpper().IndexOf("DISPONIBILIDAD")).Trim();

                                DataRow fila = ItemsList.NewRow();
                                fila[(int)ItemData.ItemCode] = p3.ItemSizeValue;//txtItemCode.Text;                                                                                         //dtInvTranl.Columns.Add("Codigo");
                                fila[(int)ItemData.Description] = description; //TxtItemName.Text;                                                                                         //dtInvTranl.Columns.Add("Descripcion");
                                fila[(int)ItemData.Quantity] = string.Format("{0:N}", Convert.ToDecimal(Quantity));                                                                      //dtInvTranl.Columns.Add("Cantidad");
                                fila[(int)ItemData.CommissionableVolumeEach] = string.Format("{0:N}", Convert.ToDecimal(Vp));                                                      //dtInvTranl.Columns.Add("Vp");
                                fila[(int)ItemData.BusinessVolumeEach] = string.Format("{0:N}", Convert.ToDecimal(Qp));                                                     //dtInvTranl.Columns.Add("Qp");
                                fila[(int)ItemData.PriceEach] = string.Format("{0:N}", _vwItemAllInfo1[0].ItemPrice.Price);                                                 //dtInvTranl.Columns.Add("priceEach");
                                fila[(int)ItemData.priceTotal] = string.Format("{0:N}", Convert.ToDecimal(Quantity) * Convert.ToDecimal(price) / vv.Count);                //dtInvTranl.Columns.Add("priceTotal");
                                //fila[9] = "1"; // Modicicaion para ver si aplicamos el descuento 
                                fila[(int)ItemData.IsAddPromotionItemQP] = "false";
                                //fila[10] = string.Format("{0:N}", _vwItemAllInfo1[0].ItemPrice.Price * PrecioPorc);                    // Override price
                                //fila[11] = string.Format("{0:N}", Convert.ToDecimal(Quantity) * _vwItemAllInfo1[0].ItemPrice.Price * PrecioPorc);
                                fila[(int)ItemData.TaxableEach] = string.Format("{0:N}", (_vwItemAllInfo1[0].ItemPrice.Price * Convert.ToDecimal(Quantity)) - _vwItemAllInfo1[0].ItemPrice.Price * PrecioPorc * Convert.ToDecimal(Quantity));
                                //fila[3] = string.Format("{0:N}", Convert.ToDecimal(Vp) / vv.Count);                                                     //dtInvTranl.Columns.Add("Qp");
                                //fila[4] = string.Format("{0:N}", Convert.ToDecimal(Quantity) / vv.Count);                                                             //dtInvTranl.Columns.Add("Vp");
                                ItemsList.Rows.Add(fila);

                            }
                            #endregion
                        }
                    });

                    #endregion
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        [WebMethod]
        public static List<string> GetItem(string ItemName)
        {
            List<string> ItemsDescription = new List<string>();

            //List<BackOfficeV2.Clases.Item> _vwItemAllInfo = ClItem.ClSearchItemByDescription(ItemName);
            List<BackOfficeV2.Clases.vwItemAllInfo> _vwItemAllInfo = BasePage._vwItemAllInfo.Where(where => where.Description.Contains(ItemName)).ToList();

            if (_vwItemAllInfo.TieneValores())
            {
                _vwItemAllInfo.ForEach(item =>
                {
                    ItemsDescription.Add(item.Description);
                });
            }
            else
            {
                ItemsDescription.Add("Items Not Found");
            }

            return ItemsDescription;
        }

        #endregion
        #region Pedido

        [WebMethod]
        public static List<BackOfficeV2.Clases.vwPedido> SaveOrder(string CityId, string ZipCode, string PaymentType)
        {
            try
            {
                #region Save Order


                if (!BasePage.CurrentCustomer.TieneValor() || BasePage.CurrentCustomer.Customer_id == 1)
                {
                    List<BackOfficeV2.Clases.vwPedido> _vwPedidoResponse2 = new List<BackOfficeV2.Clases.vwPedido>();
                    BackOfficeV2.Clases.vwPedido _vwPedidoModel = new BackOfficeV2.Clases.vwPedido();

                    _vwPedidoModel.CustomerName = "SessionEnd";
                    _vwPedidoResponse2.Add(_vwPedidoModel);

                    return _vwPedidoResponse2;
                }


                BackOfficeV2.Clases.PaymentType _PaymentType = ClmasterData.ClgetPaymentType(Convert.ToInt32(PaymentType));

                if (!PaymentType.TieneValor() && PaymentType.Equals("") && !_PaymentType.TieneValor())
                {
                    throw new Exception(CurrentLanguage.Languageid == 0 ? "Tipo de Pago No valido" : "Payment Type Not valid");
                }


                BackOfficeV2.Clases.Pedido _Order = new BackOfficeV2.Clases.Pedido();
                Int32 OrderExigo = 0;
                _Order.IdPedidoExigo = 0;
                _Order.IdWarehouse = CurrentWarehouse.WarehouseId;
                _Order.CustomerId = CurrentCustomer.Customer_id;
                _Order.CustomerName = CurrentCustomer.First_name + " " + CurrentCustomer.Last_name;
                _Order.IdFiscal = CurrentCustomer.Tax_id;
                _Order.IdCurrency = CurrentCurrency.Id;
                _Order.IdShipMethod = Convert.ToInt32(_PedidoSession.IdShipMethod);
                _Order.ShipAmount = _PedidoSession.ShipAmount;
                _Order.PaymentType = _PaymentType;
                _Order.OrderCreated = DateTime.Now;
                _Order.OrderDate = DateTime.Now;
                _Order.CreatedBy = CurrentApiAuthentication.LoginName;
                _Order.Address1 = CurrentCustomer.Main_address1;
                _Order.Address2 = CurrentCustomer.Main_address2;
                _Order.City_id = CityId;
                _Order.IdState = Convert.ToInt32(_PedidoSession.IdState);
                _Order.IdCountry = CurrentCountry.Id;
                _Order.IdOrderType = Convert.ToInt32(_PedidoSession.IdOrderType);
                _Order.OrderType = "Customer Service";
                _Order.OrderTypeName = "Customer Service";
                _Order.Phone = CurrentCustomer.Phone;
                _Order.IsActive = true;
                _Order.IdPriceType = CurrentPriceType.PriceTypeId;
                _Order.Notes = ModuloArdyssLife == "0" ? "Shopping Cart" : "Enroll" + " ArdyssLife Ip : " + HttpContext.Current.Request.ServerVariables["remote_addr"];
                _Order.TotalTax = _PedidoSession.TotalTax;
                _Order.IdStatus = _PedidoSession.IdStatus;
                _Order.IdPaymentType = _PedidoSession.IdPaymentType;
                _Order.ZipCode = ZipCode;
                _Order.FirstName = CurrentCustomer.First_name;
                _Order.LastName = CurrentCustomer.Last_name;
                _Order.Status = ClmasterData.ClSearchOrderStatus("pending")[0];
                if (CurrentWarehouse.TieneValor() && CurrentWarehouse.IdForAccounting.NotieneBlancos())
                {
                    _Order.OrderDate = CurrentWarehouse.IdForAccounting.ConvertFecha();
                }

                bool GuardarExigo = CurrentWarehouse.WarehouseId == 174 ? false : true;

                List<BackOfficeV2.Clases.Pedido_p> _Pedido_p = new List<BackOfficeV2.Clases.Pedido_p>();

                _Order.Pedido_l = _PedidoSession.Pedido_l;
                _PedidoResponse = Clpedido.GuardarPedidoSinExigo(_Order, out OrderExigo, GuardarExigo);
                _PedidoResponse[0].Pedido_l = _PedidoSession.Pedido_l;
                _PedidoSession = _PedidoResponse[0];
                _PedidoSessionModel = _PedidoResponse;
                RemoveAllViewCart();

                #endregion

            }
            catch (Exception)
            {
                throw;
            }


            return _PedidoResponse;
        }

        [WebMethod]
        public static void NewOrder()
        {
            try
            {
                for (int i = ItemsList.Rows.Count - 1; i >= 0; i--)
                {
                    ItemsList.Rows.RemoveAt(i);
                }

                _PedidoSession = new BackOfficeV2.Clases.vwPedido();
                _PedidoSessionModel = new List<BackOfficeV2.Clases.vwPedido>();
                CurrentDataItemShipping = null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected static void ProcessPayment(string OrderId)
        {
            try
            {
                /*Data.ExigoApi.GetOrdersResponse _CurrentOrder = ((Data.ExigoApi.GetOrdersResponse)ViewState["Currentorder"]);
            

                //if (TextCardNumber.Text != "4111111111111111")
                if (TextCardNumber.Text != "4111111111111111")
                {
                     BackOfficeV2.Clases.Pedido _Pedido = new BackOfficeV2.Clases.Pedido();
                     string MerchantAuthorizedCharge = ExecuteMerchant(_CurrentOrder.Orders[0].ShipMethodID, _CurrentOrder.Orders[0].Total, _CurrentOrder.Orders[0].OrderID, _log);
                     _log.CardResult = MerchantAuthorizedCharge;
                     _log.Messages = "Replicated new " + MerchantAuthorizedCharge;
                     if (MerchantAuthorizedCharge.TieneValor() && !MerchantAuthorizedCharge.Contains("no merchant configured") && !MerchantAuthorizedCharge.Contains("invalid merchant account") && !MerchantAuthorizedCharge.Contains("XML"))//&& !payment.Cardnumber.Equals("4111111111111111"))
                     {
                          var reqpago = new CreatePaymentCreditCardRequest();
                          reqpago.OrderID = _CurrentOrder.Orders[0].OrderID;//respuesta.OrderID;
                          reqpago.PaymentDate = DateTime.Today;
                          reqpago.Amount = _CurrentOrder.Orders[0].Total;
                          reqpago.CreditCardNumber = TextCardNumber.Text;
                          BackOfficeV2.Clases.CreditCardType _CreditCardType = ClmasterData.ClSearchCreditCardTypeById(Convert.ToInt32(ddlCreditCardType.SelectedValue));

                          reqpago.CreditCardType = _CreditCardType.IdExigo;
                          reqpago.ExpirationMonth = Convert.ToInt32(ddlExpireMonth.SelectedValue);
                          reqpago.ExpirationYear = Convert.ToInt32(ddlExpireyear.SelectedValue);
                          reqpago.BillingName = TextBillingFirstName + " " + TextBillingLastName;
                          reqpago.BillingAddress = TextBillingAddress1.Text;
                          reqpago.BillingCity = TextBillingCity.Text;
                          reqpago.BillingZip = TextBillingSipCode.Text;
                          reqpago.AuthorizationCode = MerchantAuthorizedCharge; // TODO: Numero de Autorizacion Revisar  pagos[p].Autorization_number;
                          CreatePaymentCreditCardResponse Createapaymentccresponse = Exigo.CreatePagoCc(reqpago);
                          Data.ExigoApi.ChangeOrderStatusRequest ReqStatus = new Data.ExigoApi.ChangeOrderStatusRequest();
                          ReqStatus.OrderID = _CurrentOrder.Orders[0].OrderID;
                          ReqStatus.OrderStatus = OrderStatusType.Accepted;
                          Exigo.ChangeOrderStatus(ReqStatus, 8, null);
                          ClmasterData.GuardarLogCc(_log);

                          Int32 Pedido = _CurrentOrder.Orders[0].OrderID;

                          lblPaymentAcepted.Text = "Acepted";
                          lblAutorizationNUmber.Text = MerchantAuthorizedCharge;
                          labelOrderAmount.Text = _CurrentOrder.Orders[0].Total.ToString();
                          LabelOrderNumber.Text = _CurrentOrder.Orders[0].OrderID.ToString();
                          // ToDo Borrar los balores de las sessiones ejemplo las lineas 
                          Btnretry.Visible = false;
                          MultiView1.ActiveViewIndex = 6;
                     }
                     else
                     {
                          //TODO: mostrar el error en esta pagina. y decirle que reintente.
                          // Guardar Log de la transaccion 
                          ClmasterData.GuardarLogCc(_log);
                          lblPaymentAcepted.Text = "Denied";
                          lblAutorizationNUmber.Text = MerchantAuthorizedCharge;
                          labelOrderAmount.Text = _CurrentOrder.Orders[0].Total.ToString();
                          LabelOrderNumber.Text = _CurrentOrder.Orders[0].OrderID.ToString();
                          Btnretry.Visible = true;
                          //                                throw new Exception(MerchantAuthorizedCharge);
                     }
                }
                */
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Customer

        [WebMethod]
        public static BackOfficeV2.Clases.CustomerSite GetCustomerSiteForLogin(string User, String Password)
        {

            BackOfficeV2.Clases.CustomerSite _CustomerSite = null;

            try
            {

                if (!CurrentCustomer.TieneValor() || CurrentCustomer.Customer_id == 1)
                {
                    _CustomerSite = new BackOfficeV2.Clases.CustomerSite();
                    _CustomerSite.Notes1 = "SessionEnd";

                    throw new Exception("SessionEnd");
                }

                #region verisar si esta correcto el usuario y Password
                AuthenticateCustomerRequest req = new AuthenticateCustomerRequest();
                req.LoginName = User;
                req.Password = Password;
                AuthenticateCustomerResponse res = Exigo.AuthenticateCustomer(req);

                if (!res.TieneValor())
                {
                    throw new Exception(CurrentLanguage.Languageid == 1 ? "Error en usuario y Password" : "Error User and password");
                }
                else
                {
                    SetCurrentCustomer(res.CustomerID);
                    _CustomerSite = ClCustomer.GetCustomerSiteByCustomerId(res.CustomerID);


                    if (_CustomerSite.TieneValor())
                    {
                        setWebAlias(_CustomerSite.WebAlias);
                    }
                }

                return _CustomerSite;
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }

        [WebMethod]
        public static void CreateCustomer(string languageId,
                                                        string firstName, string lastName, string genderId,
                                                        string companyName,
                                                        string taxIdType, string taxId,
                                                        string birthDay, string birthMonth, string birthYear,
                                                        string address, string address2,
                                                        string billingAddress, string billingAddress2, string billingStateId, string city, string zipCode,
                                                        string phone, string fax, string eMail,
                                                        string backOfficeUserName, string pass, string replicatedUserName,
                                                        string idPaymentType, string paymentType, Int32 CustomerType, int Module)
        {
            try
            {


                List<BackOfficeV2.Clases.Customer> _CustomerEmail = ClCustomer.SearchCustomerByEmail(eMail);
                GetCustomerSiteResponse resWebAlias = null;

                if (_CustomerEmail.TieneValores())
                {
                    throw new Exception(CurrentLanguage.Languageid == 1 ? string.Format("El correo <b> {0} </b> ya existe", eMail) : string.Format("Email  <b> {0} </b>  Already Exist ", eMail));
                }

                if (!backOfficeUserName.Equals(""))
                {
                    List<BackOfficeV2.Clases.vwCustomer> _CustomerUser = ClCustomer.SearchCustomersByLogin(backOfficeUserName);
                    if (_CustomerUser.TieneValores())
                    {
                        throw new Exception(CurrentLanguage.Languageid == 1 ? string.Format("El usuario <b>  {0}  </b> No esta disponible", backOfficeUserName) : string.Format("User <b> {0} </b> No Available ", backOfficeUserName));
                    }
                }

                CreateCustomerRequest req = new CreateCustomerRequest();

                req.FirstName = firstName;
                req.LastName = lastName;
                req.Company = companyName.Equals(string.Empty) ? null : companyName;
                req.CustomerType = CustomerType;
                req.CustomerStatus = 1;
                req.Email = eMail;
                req.Phone = phone;
                req.MainAddress1 = billingAddress;
                req.MainCountry = CurrentCountry.CountryCode;
                req.MainState = ClmasterData.ClgetStateById(Convert.ToInt32(billingStateId)).StateCode;
                req.MainCity = city;
                req.MainZip = zipCode;
                req.MailAddress1 = address;
                req.MailCountry = CurrentCountry.CountryCode;
                req.MailState = ClmasterData.ClgetStateById(Convert.ToInt32(billingStateId)).StateCode;
                req.MailCity = city;
                req.MailZip = zipCode;
                req.InsertEnrollerTree = true;
                req.EnrollerID = CurrentCustomer.Customer_id;
                req.InsertUnilevelTree = true;
                req.SponsorID = CurrentCustomer.Customer_id;
                req.TaxID = taxId;
                req.SalesTaxID = taxId;
                req.Field12 = "1";
                req.CurrencyCode = CurrentCurrency.CurrencyCode;
                req.DefaultWarehouseID = CurrentWarehouse.WarehouseId;

                if (Module == Convert.ToInt32(ModuleData.Enroll))
                {
                    req.CanLogin = true;
                    req.Fax = fax;
                    req.BirthDate = new DateTime(Convert.ToInt32(birthYear), Convert.ToInt32(birthMonth), Convert.ToInt32(birthDay));
                    req.LoginName = backOfficeUserName;
                    req.LoginPassword = pass;
                    req.MailAddress2 = address2;
                    req.MainAddress2 = billingAddress2;
                    req.LanguageID = Convert.ToInt32(languageId);
                    req.Gender = (Data.ExigoApi.Gender)Convert.ToInt32(genderId);
                }



                //TODO: No existe el 12 y es necesario para mexico
                switch (taxIdType.Equals("") ? 0 : Convert.ToInt32(taxIdType))
                {
                    case 1:
                        req.TaxIDType = TaxIDType.SSN;
                        break;
                    case 2:
                        req.TaxIDType = TaxIDType.EIN;
                        break;
                    case 3:
                        req.TaxIDType = TaxIDType.OtherType4;
                        break;
                    case 4:
                        req.TaxIDType = TaxIDType.OtherType5;
                        break;
                    case 5:
                        req.TaxIDType = TaxIDType.OtherType6;
                        break;
                    case 6:
                        req.TaxIDType = TaxIDType.OtherType7;
                        break;
                    case 7:
                        req.TaxIDType = TaxIDType.OtherType8;
                        break;
                    case 8:
                        req.TaxIDType = TaxIDType.OtherType9;
                        break;
                    case 9:
                        req.TaxIDType = TaxIDType.OtherType10;
                        break;
                }

                CreateCustomerResponse res = Exigo.CreateCustumerOnExigo(req);

                if (Module == Convert.ToInt32(ModuleData.Enroll))
                {
                    Exigo.ChangeCustomerSite(res.CustomerID, backOfficeUserName, req.FirstName, req.LastName, req.Email);
                    UpdateCustomerRequest UpdateCustReq = new UpdateCustomerRequest();
                    UpdateCustReq.CustomerID = res.CustomerID;
                    UpdateCustReq.CanLogin = true;
                    UpdateCustReq.LoginName = backOfficeUserName;
                    UpdateCustReq.LoginPassword = pass;
                    Exigo.UpdateCustomerRequest(UpdateCustReq);
                    Exigo.GetCustomerToDBFromExigo(res.CustomerID);
                }
                else
                {
                    Exigo.ChangeCustomerSite(res.CustomerID, res.CustomerID.ToString(), req.FirstName, req.LastName, req.Email);
                    UpdateCustomerRequest UpdateCustReq = new UpdateCustomerRequest();
                    UpdateCustReq.CustomerID = res.CustomerID;
                    UpdateCustReq.CanLogin = true;
                    UpdateCustReq.LoginName = res.CustomerID.ToString();
                    UpdateCustReq.LoginPassword = res.CustomerID.ToString();
                    Exigo.UpdateCustomerRequest(UpdateCustReq);
                    Exigo.GetCustomerToDBFromExigo(res.CustomerID);
                }

                CurrentCustomer = Exigo.GetCustomerToDBFromExigo(res.CustomerID);

                _PedidoSession.Address1 = CurrentCustomer.Main_address1;
                _PedidoSession.Address2 = CurrentCustomer.Main_address2;
                _PedidoSession.CustomerId = CurrentCustomer.Customer_id;
                _PedidoSession.CustomerName = CurrentCustomer.First_name + " " + CurrentCustomer.Last_name;
                _PedidoSession.FirstName = CurrentCustomer.First_name;
                _PedidoSession.IdFiscal = CurrentCustomer.Tax_id;
                _PedidoSession.IdPaymentType = Convert.ToInt32(idPaymentType);
                _PedidoSession.LastName = CurrentCustomer.Last_name;
                _PedidoSession.paymentTypeName = paymentType;
                _PedidoSession.Phone = CurrentCustomer.Phone;



            }
            catch (Exception)
            {
                throw;
            }
        }

        [WebMethod]
        public static bool isUserNameAvailable(string kind, string userName, string pass)
        {
            if (kind.Equals("BackOffice"))
            {
                AuthenticateCustomerRequest req = new AuthenticateCustomerRequest();

                req.LoginName = userName;
                req.Password = pass;

                try
                {
                    Exigo.AuthenticateCustomer(req);

                    return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }
            else
            {
                return !ClCustomer.ClgetCustomerSiteByWebAlias(userName).TieneValor();
            }
        }

        #endregion
        #region Enroll

        [WebMethod]
        public static BackOfficeV2.Clases.vwSubscriptionTypeCountry getSubscriptionTypeSelect()
        {
            try
            {
                return SelectedSubscription;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

    }
}