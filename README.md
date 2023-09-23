   # HiKari使用说明 
郑重说明：本库HiKari定位于数据库连接池，源码GitHub开源公开，欢迎大家修改提交  
## 程序初始化
使用配置类
```
      HikariConfig hikariConfig = new HikariConfig();
                   hikariConfig.DBType = "PostgreSQL";
                   hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";
                   hikariConfig.DriverDir = "DBDrivers";
                   hikariConfig.DriverDLL = "XXXX.dll";
                   hikariConfig.DBTypeXml = "DBType.xml";
      HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);
```

配置文件（推荐方式,配置字段就是HikariConfig的属性):

```
HikariConfig hikariConfig = new HikariConfig();
hikariConfig.LoadConfig("Hikari.txt");
HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);

```

日志框架（默认Microsoft.Extensions.Logging,用Serilog替换Serilog.Extensions.Logging):

```
 Log.Logger = new LoggerConfiguration().
                MinimumLevel.
                Debug().
                Enrich.
                FromLogContext().
                WriteTo.Console(new JsonFormatter()).CreateLogger();
  SerilogLoggerProvider provider = new SerilogLoggerProvider(Log.Logger);
          //  LoggerProviderCollection providerCollection = new LoggerProviderCollection();
           // providerCollection.AddProvider(provider);
           // var factory = new SerilogLoggerFactory(null, true, providerCollection);
           // HikariLogger hikariLogger=new HikariLogger(factory);
            HikariLogger hikariLogger1 = new HikariLogger(provider.CreateLogger("HikariLogger"));
            Hikari.Logger.Singleton.HKLogger = hikariLogger1;
            //Hikari.Logger.Singleton.HKLogger = hikariLogger;

```
使用连接
hikariDataSource.GetConnection();  
使用批量处理接口
var bulk= hikariDataSource.GetBulkCopy();  
说明：数据库提供的专门用于批量插入的类需要底层客户端驱动提供，所以增加了接口，满足调用这些类处理批量插入  

使用多库管理类  
例如：配置文件MySql_HiKari.cfg    
ManagerPool.Singleton.GetDbConnection(MySql);   
ManagerPool.Singleton.GetBulkCopy(MySql);   



操作数据库
```
            string sql = "select * from  person where id=@ID";  
            Dictionary<string, object> dic = new Dictionary<string, object>();  
            dic["ID"] = 1;  
            HikariConfig hikariConfig = new HikariConfig();  
            hikariConfig.DBType = "PostgreSQL";  
            hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";  
            HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);  
            var ds = hikariDataSource.ExecuteQuery(sql, dic);  

               Dictionary<string, SqlValue> map = new Dictionary<string, SqlValue>();    
                uint[] tmp = new uint[3] {1,2, 3};
                IPAddress pAddress =  IPAddress.Parse("127.0.0.1");
                map.Add("p", new SqlValue() { Type= "IPAddress", Value= pAddress });
                map.Add("m", new SqlValue() { Type="uint[]", Value=tmp});
                hikariDataSource.ExecuteUpdate("insert into test(temp,kk)values(@p,@m)", map);

```

```
            string sql = "select * from  person";  
            var ds = ManagerPool.Singleton.ExecuteQuery(sql);  
            var dt = ds.Tables[0];  
            sql = "insert into person(id,name)values(1,'jinyu')";  
            int r = ManagerPool.Singleton.ExecuteUpdate(sql);   

```
 
使用SQLValue提供泛型，程序只需要有相同属性定义的类即可		
详细使用可以查看例子   

## 配置文件说明
1.连接字符串必须要  
2.驱动目录DriverDir项不配置则使用默认drivers目录  
3.关于驱动dll有2种：  
  （1）直接在配置文件中配置DriverDLL项，这个时候不需要DBType配置项  
  （2）DBType项和DBType.xml结合使用，这时不需要单独在每个连接配置中配置DriverDLL。  
      这个时候连接池查找dll的方式：  
      1>DBType配置是默认的4类，则无需DBType.xml来查找dll文件名称。  
      2>DBType配置不是默认的4类，则需要BType.xml中的配置来获取DLL文件名称。因为程序不知道dll.  
       其实这2种就是分散与汇总的区别。有的习惯每个配置文件配置dll,方便监测；有的习惯写在一起，方便管理。看自己情况。  
       DBType程序中写死了4种，见附录。  
 DBType.xml我称为全局配置文件，读取时的配置路径和名称可以设置HikariConfig或者HikariDataSource的DBTypeXml属性进行修改；默认就是DBType.xml  
4.增加驱动特殊类型配置：  
  （1）使用ParameterType配置文件路径，ParameterType.xml默认文件。
   (2）文件按照数据库配置，节点下面是字段转换说明。例如：npgl驱动中，uint数组对应数据库中oidvector类型。
   <Npgsql>
		<oidvector>uint[]</oidvector>
  </Npgsql>


-------------------------------------------------------------------------------------------------------

## 源码简说
### 基本内容
HikariDataSource 对外提供连接  
HikariConfig 对外配置  
HikariPool 管理操作集合，连接来源  
PoolBase 操作驱动连接，是HikariPool父类  

----------------------------------------------------------------


## 附录
|数据库	|Dll名称|说明|
|:-------:|:------:|:-----:|
|Oracle	|Oracle.ManagedDataAccess	|以前不是这个dll|
|MySql	|MySql.Data| 
|SqlServer|System| |
|PostgreSQL|Npgsql||
