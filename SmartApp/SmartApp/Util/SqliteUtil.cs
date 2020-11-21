using System;
using System.Collections.Generic;
using System.Text;
using SmartApp.Model;
using SQLite;
using Xamarin.Forms;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;

namespace SmartApp
{
    public class SqliteUtil
    {
        static SqliteUtil baseSqlite;
        public static SqliteUtil Current
        {
            get { return baseSqlite ?? (baseSqlite = new SqliteUtil()); }
        }
        private static readonly SQLiteAsyncConnection db;
        static SqliteUtil()
        {
            if (db == null)
                db = DependencyService.Get<ISQLite>().GetAsyncConnection();
        }
        public async void CreateAllTablesAsync()
        {
            try
            {
                await db.CreateTablesAsync<CataLogInfo, IamgeSaveInfo, ImageColorInfo>();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #region CataLogInfo
        public async Task<int> DeleteCataLogInfoByCateLogId(string CateLogId)
        {
            return await db.ExecuteAsync("DELETE FROM CataLogInfo WHERE CateLogId ='" + CateLogId + "'");
        }
        public async Task<int> DeleteCataLogInfo()
        {
            return await db.ExecuteAsync("DELETE FROM CataLogInfo");
        }
        public async Task<List<Model.CataLogInfo>> QueryCataLogInfoByCateLogId(int CateLogId)
        {
            return await db.QueryAsync<Model.CataLogInfo>("SELECT * FROM CataLogInfo WHERE CateLogId ='" + CateLogId.ToString() + "'");
        }
        public async Task<Model.CataLogInfo> QueryCataLog(int id)
        {
            return await db.Table<Model.CataLogInfo>().Where(a => a.CateLogId == id).FirstOrDefaultAsync();
        }
        public async Task<List<Model.CataLogInfo>> QueryCataLogs(int pageSize)
        {
            return await db.Table<Model.CataLogInfo>().OrderByDescending(a => a.CreateDatetime).Skip(0).Take(pageSize).ToListAsync();
        }

        public async Task<List<Model.CataLogInfo>> QueryGetAll()
        {
            return await db.QueryAsync<Model.CataLogInfo>("SELECT * FROM CataLogInfo");
        }

        public async Task<List<Model.CataLogInfo>> QueryCataLogsByTitle(string strTitle)
        {
            return await db.Table<Model.CataLogInfo>().Where(a => a.Title == strTitle).OrderByDescending(a => a.CreateDatetime).Skip(0).Take(10).ToListAsync();
        }
        public async Task<List<Model.CataLogInfo>> QueryCataLogsByFolder(string strFolder)
        {
            return await db.Table<Model.CataLogInfo>().Where(a => a.Folder == strFolder).OrderByDescending(a => a.CreateDatetime).Skip(0).Take(10).ToListAsync();
        }
        public async Task UpdateCataLogs(List<Model.CataLogInfo> lists)
        {
            foreach (var item in lists)
            {
                await QueryCataLog(item.CateLogId).ContinueWith(async (results) =>
                {
                    if (results.Result == null)
                    {
                        try
                        {
                            await db.InsertAsync(item);
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    }
                    else
                    {
                        await UpdateCataLog(item);
                    }
                });
            }
        }
        public async Task<int> UpdateCataLog(Model.CataLogInfo model)
        {
            int ret = -1;
            try
            {
                ret = await db.UpdateAsync(model);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return ret;
        }

        public async Task<int> InsertCataLog(Model.CataLogInfo model)
        {
            int ret = -1;
            try
            {
                ret = await db.InsertAsync(model);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return ret;
        }
        #endregion

        #region IamgeSaveInfo
        public async Task<int> DeleteIamgeSaveInfo()
        {
            return await db.ExecuteAsync("DELETE FROM IamgeSaveInfo");
        }
        public async Task<int> DeleteIamgeSaveInfoByImageID(string imageID)
        {
            return await db.ExecuteAsync("DELETE FROM IamgeSaveInfo WHERE ImageID ='" + imageID + "'");
        }
        public async Task<List<Model.IamgeSaveInfo>> QueryIamgeSaveInfo()
        {
            return await db.QueryAsync<Model.IamgeSaveInfo>("SELECT * FROM IamgeSaveInfo");
        }
        public async Task<List<Model.IamgeSaveInfo>> QueryIamgeSaveInfoByImageID(int imageID)
        {
            return await db.QueryAsync<Model.IamgeSaveInfo>("SELECT * FROM IamgeSaveInfo WHERE ImageID ='" + imageID.ToString() + "'");
        }
        public async Task<List<Model.IamgeSaveInfo>> QueryIamgeSaveInfoByCateLogId(int CateLogId)
        {
            return await db.QueryAsync<Model.IamgeSaveInfo>("SELECT * FROM IamgeSaveInfo WHERE CateLogId ='" + CateLogId.ToString() + "'");
        }
        public async Task<List<Model.IamgeSaveInfo>> QueryIamgeSaveInfoByImagePath(string ImagePath)
        {
            return await db.QueryAsync<Model.IamgeSaveInfo>("SELECT * FROM IamgeSaveInfo WHERE ImagePath ='" + ImagePath + "'");
        }

        public async Task<Model.IamgeSaveInfo> QueryImage(int id)
        {
            return await db.Table<Model.IamgeSaveInfo>().Where(a => a.ImageID == id).FirstOrDefaultAsync();
        }

        public async Task<Model.IamgeSaveInfo> QueryImageByFileName(string  filename)
        {
            return await db.Table<Model.IamgeSaveInfo>().Where(a => a.ImagePath == filename).FirstOrDefaultAsync();
        }


        public async Task<List<Model.IamgeSaveInfo>> QueryImages(int pageSize)
        {
            return await db.Table<Model.IamgeSaveInfo>().OrderByDescending(a => a.Createdatetime).Skip(0).Take(pageSize).ToListAsync();
        }        
        public async Task UpdateImageInfos(List<Model.IamgeSaveInfo> lists)
        {
            foreach (var item in lists)
            {
                await QueryCataLog(item.ImageID).ContinueWith(async (results) =>
                {
                    if (results.Result == null)
                    {
                        try
                        {
                            await db.InsertAsync(item);
                        }
                        catch (Exception ex)
                        {
                            Crashes.TrackError(ex);
                        }
                    }
                    else
                    {
                        await UpdateImage(item);
                    }
                });
            }
        }
        public async Task UpdateImage(Model.IamgeSaveInfo model)
        {
            try
            {
                await db.UpdateAsync(model);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task<int> InsertImageInfo(Model.IamgeSaveInfo model)
        {
            int ret = -1;
            try
            {

                ret = await db.InsertAsync(model);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return ret;
        }

        #endregion

        #region ImageColorInfo
        public async Task<int> DeleteImageColorInfoByImageID(string imageID)
        {
            return await db.ExecuteAsync("DELETE FROM ImageColorInfo WHERE ImageID ='" + imageID + "'");
        }
        public async Task<int> DeleteImageColorInfo()
        {
            return await db.ExecuteAsync("DELETE FROM ImageColorInfo");
        }
        public async Task<List<Model.ImageColorInfo>> QueryMaxImageIdInfo()
        {
            return await db.QueryAsync<Model.ImageColorInfo>("SELECT max(ImageID) FROM ImageColorInfo");
        }
        public async Task<List<Model.ImageColorInfo>> QueryImageColorInfo()
        {
            return await db.QueryAsync<Model.ImageColorInfo>("SELECT * FROM ImageColorInfo");
        }

        public async Task<List<Model.ImageColorInfo>> QueryImageColorInfoByImageID(int imageID)
        {
            return await db.QueryAsync<Model.ImageColorInfo>("SELECT * FROM ImageColorInfo WHERE ImageID ='" + imageID.ToString() +"'");
        }

        public async Task<List<Model.ImageColorInfo>> QueryImageColorInfoByColorID(int colorID)
        {
            return await db.QueryAsync<Model.ImageColorInfo>("SELECT * FROM ImageColorInfo WHERE ColorId ='" + colorID.ToString() + "'");
        }

        public async Task<int> InsertImageColorInfo(Model.ImageColorInfo model)
        {
            int ret = -1;
            try
            {
                ret = await db.InsertAsync(model);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return ret;
        }
        #endregion

    }
}
