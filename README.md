   # HiKari使用说明
已经提交nuget(Hikari)   
郑重说明：本库HiKari定位于数据库连接池，源码GitHub开源公开，欢迎大家修改提交，但是不得对本库扩展，所有修改仅限于修改bug,性能优化，存储数据结构优化重构等。一切需要扩展的功能都需要您单独建库（项目)，建议以Hikari.Integration.XXXX(名称)库，引用当前库进行扩展。
## 程序初始化
  使用配置类
```
      HikariConfig hikariConfig = new HikariConfig();
                   hikariConfig.DBType = "PostgreSQL";
                   hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database =      postgres;Pooling=true; ";
                   hikariConfig.DriverDir = "DBDrivers";
                   hikariConfig.DriverDLL = "XXXX.dll";
                   hikariConfig.DBTypeXml = "DBType.xml";
      HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);
```
直接使用：
```
  HikariDataSource hikariDataSource = new HikariDataSource();
                   hikariDataSource.DBType = "PostgreSQL";
                   hikariDataSource.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";
                   hikariDataSource.DriverDir = "DBDrivers";
                   hikariDataSource.DriverDLL = "XXXX.dll";
                   hikariDataSource.DBTypeXml = "DBType.xml";
```
配置文件（推荐方式):

```
HikariConfig hikariConfig = new HikariConfig();
hikariConfig.LoadConfig("Hikari.txt");
HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);

```
使用连接
hikariDataSource.GetConnection();  

使用批量处理接口
var bulk= hikariDataSource.GetBulkCopy();  
说明：我的博文中总结了几种批量处理的方式，唯独数据库提供的专门用于批量插入的类需要底层客户端驱动提供，所以增加了接口，满足调用这些类处理批量插入  

使用多库管理类
ManagerPool.Singleton.GetDbConnection(MySql); 
ManagerPool.Singleton.GetBulkCopy(MySql);  

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
## 升级内容
2018-12-13  
1.优化加载，对全局配置文件DBType.xml和默认项进行检查，已经加载过的就不加载了。  
2.新增连接池管理。可以同时启用多个连接池。连接池使用和以前一样。但是可以同时使用多个。根据配置文件名称来获取不同的连接池连接。只是这个时候只能能够使用配置文件，而不能在使用配置类了。  
连接池管理类：ManagerPool  
ManagerPool获取连接时传入一个名称（配置文件名称）来获取连接，这时不同的配置名称就意味着不同的连接池。  
另外该类提供了驱动目录和全局配置设置。方便把多个连接池使用到的所以DLL放在一起方便管理。连接池管理类不会覆盖已经在各个连接池配置文件配置的DriverDir目录，但是DBTypeXml属性会覆盖，只能有个，并且在连接池管理类中设置。  
同时提供了使用的连接管理，该管理根据获取连接的线程保存连接对象。如果设置了线程关闭，同一个线程获取新连接，则以前的连接将会关闭。你也可以在获取的连接的线程中再次获取该对象。  
  
2018-12-31  
增加SqlServer连接从运行时环境中加载。因为sqlserver的驱动连接包含在.NET类库中（System.Data.dll），所以加载了一次运行时环境。
如果你没有提供该dll在驱动目录中，根据DBType或者dll名称判断是SqlServer连接，则会去查找一次.NET按照目录获取客户端驱动。

--------------------------------------------------------------------------------------------------------

连接池管理类：连接池管理中的配置文件名称进行了修改。除非是默认枚举名称Hikari没有变，如果你使用了名称，则会自动添加_Hikari。文件类型变成了cfg。例如：使用MySql,那么配置文件应该是MySql_Hikari.cfg.添加后缀的主要原因是其它一些数据库配置组件也使用配置文件，能够全部放在一个目录下。毕竟使用连接池管理，配置文件需要一定规律查找。如果不愿意只需改变管理类中的文件名称查找，很简单。  

连接池管理管理类使用：
例如：配置文件MySql_HiKari.cfg  
ManagerPool.Singleton.GetDbConnection(MySql);


2019-03-30  

    没有bug修改，只是增加默认参数值，完善代码
	
2019-04-01

   扩展封装使用，可以直接进行SQL语句操作  
   通过HikariDataSource的扩展方法  
   通过ManagerPool的扩展方法  

-------------------------------------------------------------------------------------------------------


## 源码简说
### 基本内容
HikariDataSource 对外提供连接  
HikariConfig 对外配置  
HikariPool 管理操作集合，连接来源  
PoolBase 操作驱动连接，是HikariPool父类  
### 新增扩展内容
2018-12-13  
ManagerPool 线程池管理类，根据名称提供多线程连接  

2018-12-32  
增加SqlServer连接从运行时环境中加载。因为sqlserver的驱动连接包含在.NET类库中，所以加载了一次运行时环境。
如果你使用没有提供。根据DBType或者dll名称判断是SqlServer连接，则会去查找一次.NET按照目录获取客户端驱动。  

2019-01-23  
Hikari.Integration.Models 新增  
     扩展对datatable,datareader转换List<T>,支持反射，表达式转换    
Hikari.Integration.Models.Core 新增   
     扩展.net core库emit方法对datatable,datareader转换List<T>  
Hikari.Integration.Models.Emit 新增   
     扩展.net framework库emit方法对datatable,datareader转换List<T>  
 
2019-02-15

 新增数据库驱动批量处理类的接口  
           var bulk=   hikariDataSource.GetBulkCopy();  
            bulk.BulkCopy(dt);    
2019-02-21  

    新增CheckSQL方法，执行ConnectionInitSql配置的SQL语句
    在HikariDataSource和ManagerPool都有该方法  
	
2019-02-25  

Hikari.Integration.Models.Core    
      扩展.net core库emit方法对List<T>转换DataTable  
Hikari.Integration.Models.Emit   
      扩展.net framework库emit方法对List<T>转换DataTable   
	  
2019-03-24  

Hikari.Integration.Entity 
      扩展库emit方法DataTabble,DataReader与List<T>相互转换  
	  该扩展内部引用EntityMappingDBEmit项目，替换现有所有扩展库


   
----------------------------------------------------------------
说明：该项目的扩展项目Hikari.Integration.Models.Core 和Hikari.Integration.Models.Emit  已经被EntityMappingDBEmit项目代替

## 附录
|数据库	|Dll名称|说明|
|:-------:|:------:|:-----:|
|Oracle	|Oracle.ManagedDataAccess	|以前不是这个dll|
|MySql	|MySql.Data| 
|SqlServer|System| |
|PostgreSQL|Npgsql||
