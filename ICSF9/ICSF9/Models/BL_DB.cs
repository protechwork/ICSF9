using System;
using System.Data;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ICSF9.Models
{
    public class BL_DB
    {
        public int GetExecute(string strInsertOrUpdateQry, int CompId, ref string error)
        {
            try
            {
                Database obj = Focus.DatabaseFactory.DatabaseWrapper.GetDatabase(CompId);
                return (obj.ExecuteNonQuery(CommandType.Text, strInsertOrUpdateQry));
            }
            catch (Exception e)
            {
                error = e.Message;
                return 0;
            }
        }
        public DataSet GetData(string strSelQry, int CompId, ref string error)
        {
            try
            {
                
                clsGeneric.writeLog( "Query : " + strSelQry);
                Database obj = Focus.DatabaseFactory.DatabaseWrapper.GetDatabase(CompId);
                return (obj.ExecuteDataSet(CommandType.Text, strSelQry));
            }
            catch (Exception e)
            {

                clsGeneric.writeLog("Exception : " + e);
                error = e.Message;
                return null;
            }
        }
    }
}