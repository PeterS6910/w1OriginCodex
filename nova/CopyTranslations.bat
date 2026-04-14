@echo off
 
set mainPath="\\VMFILESERVER\ContalRepository\TRANSLATIONS\Contal Nova"

echo F|xcopy Cgp.NCAS.Client\Localization.English.resx %mainPath%\Cgp.NCAS.Client\Localization.English.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp.NCAS.Client\Localization.Swedish.resx %mainPath%\Cgp.NCAS.Client\Localization.Swedish.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp.NCAS.Client\Localization.Slovak.resx %mainPath%\Cgp.NCAS.Client\Localization.Slovak.resx /e/s/h/i/k/q/r/y

echo F|xcopy Cgp.NCAS.Server\Localization.English.resx %mainPath%\Cgp.NCAS.Server\Localization.English.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp.NCAS.Server\Localization.Swedish.resx %mainPath%\Cgp.NCAS.Server\Localization.Swedish.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp.NCAS.Server\Localization.Slovak.resx %mainPath%\Cgp.NCAS.Server\Localization.Slovak.resx /e/s/h/i/k/q/r/y

echo F|xcopy Cgp.NCAS.CCU\Localization\Localization.English.resx %mainPath%\Cgp.NCAS.CCU\Localization.English.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp.NCAS.CCU\Localization\Localization.Swedish.resx %mainPath%\Cgp.NCAS.CCU\Localization.Swedish.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp.NCAS.CCU\Localization\Localization.Slovak.resx %mainPath%\Cgp.NCAS.CCU\Localization.Slovak.resx /e/s/h/i/k/q/r/y

echo F|xcopy Cgp\Cgp.Client\Localization.English.resx %mainPath%\Cgp.Client\Localization.English.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp\Cgp.Client\Localization.Swedish.resx %mainPath%\Cgp.Client\Localization.Swedish.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp\Cgp.Client\Localization.Slovak.resx %mainPath%\Cgp.Client\Localization.Slovak.resx /e/s/h/i/k/q/r/y

echo F|xcopy Cgp\Cgp.Server\Localization.English.resx %mainPath%\Cgp.Server\Localization.English.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp\Cgp.Server\Localization.Swedish.resx %mainPath%\Cgp.Server\Localization.Swedish.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp\Cgp.Server\Localization.Slovak.resx %mainPath%\Cgp.Server\Localization.Slovak.resx /e/s/h/i/k/q/r/y

echo F|xcopy Cgp\Cgp.DBSCreator\Localization.English.resx %mainPath%\Cgp.DBSCreator\Localization.English.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp\Cgp.DBSCreator\Localization.Swedish.resx %mainPath%\Cgp.DBSCreator\Localization.Swedish.resx /e/s/h/i/k/q/r/y
echo F|xcopy Cgp\Cgp.DBSCreator\Localization.Slovak.resx %mainPath%\Cgp.DBSCreator\Localization.Slovak.resx /e/s/h/i/k/q/r/y