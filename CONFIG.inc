source ../CONFIG.inc

PACKAGE=AGExt
TARGETDIR="Diazo/$PACKAGE"
TARGETBINDIR="$TARGETDIR/Plugins/"
PROJECTSDIR="$PACKAGE/"

VERSION=$( cat $PACKAGE.version | tr '\n' ' ' | sed -n -E 's/^.*?"VERSION\":\{.*"MAJOR":([0-9]+?),.*?"MINOR":([0-9]+?),.*?"PATCH":([0-9]+?),.*?"BUILD":([0-9]+?).*?\}.*$/\1.\2.\3.\4/p' )
