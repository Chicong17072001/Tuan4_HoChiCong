using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using Tuan4_HoChiCong.Models;
namespace Tuan4_HoChiCong.Controllers
{
    public class GioHangController : Controller
    {
        //commit lan 2
        // GET: GioHang
        MydataDataContext data = new MydataDataContext();
        public List<GioHang> Laygiohang()
        {
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if(lstGiohang == null)
            {
                lstGiohang = new List<GioHang>();
                Session["GioHang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGiohang(int id ,string strURL)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.Find(n => n.masach == id);
            if(sanpham == null)
            {
                sanpham = new GioHang(id);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int tsl = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Sum(n => n.iSoluong);
            }
            return tsl;
        }
        private int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if(lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return tsl;
        }
        private double TongTien()
        {
            double tt = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tt = lstGiohang.Sum(n => n.dThanhtien);
            }
            return tt;
        }
        public ActionResult Giohang()
        {
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        public ActionResult GiohangPartial()
        {
            
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }
        public ActionResult XoaGiohang(int id)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.masach == id);
            if(sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.masach == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapNhatGiohang(int id, System.Web.Mvc.FormCollection collection)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.masach == id);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(collection["txtSolg"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGiohang()
        {
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("GioHang");
        }
        public ActionResult Dathang()
        {
            List<GioHang> lstGioHang = Laygiohang();
            if (lstGioHang.Count() != 0)
            {
                DialogResult result = MessageBox.Show("bạn muốn đặt hàng", "Hỏi", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    //create invoice
                    Invoice invoice = new Invoice();
                    invoice.Invoice_DateCreate = DateTime.Now;
                    data.Invoices.InsertOnSubmit(invoice);
                    data.SubmitChanges();
                    int invoide_id = data.Invoices.OrderByDescending(p => p.Invoice_ID).Select(p => p.Invoice_ID).FirstOrDefault();
                    //add invoice's detail
                    Invoice_Detail idetail;
                    foreach (var ele in lstGioHang)
                    {
                        idetail = new Invoice_Detail();
                        idetail.masach = ele.masach;
                        idetail.Invoice_ID = invoide_id;
                        idetail.giamua = ele.giaban;
                        idetail.soluong = ele.iSoluong;
                        data.Invoice_Details.InsertOnSubmit(idetail);
                        var book = data.Saches.FirstOrDefault(p => p.masach == ele.masach);
                        book.soluongton -= idetail.soluong;
                        UpdateModel(book);
                    }
                    data.SubmitChanges();
                    string str = "";
                    int i = 1;
                    foreach (var ele in lstGioHang)
                    {
                        str += i + " - " + ele.tensach + "\n";
                        i++;
                    }
                    MessageBox.Show("Đặt hàng thành công!\n" + "---------------\n" + "Danh sách đặt hàng\n" + str);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                MessageBox.Show("Giỏ hàng trống");
            }
            
            return RedirectToAction("Index", "Home");

        }
    }
}