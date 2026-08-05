pushd C:\TFS_AreaLavoro\Gias\RamoPrincipale\Src\GiasDotNet\AgronicaStampe_2010\AgronicaStampe_2010\GestioneStampe\FreshAndFood\Etichette
for /r %%a in (*.rpt) do (
COPY "%%a" "C:\TFS_AreaLavoro\Gias\RamoPrincipale\Src\GiasDotNet\AgronicaStampe_2010\AgronicaStampe_2010\GestioneStampe\FreshAndFood\Etichette\Personalizzazioni\%%~nxa"
)
popd

Pause