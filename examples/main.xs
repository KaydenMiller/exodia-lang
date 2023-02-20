class Point {
    fn constructor() {
        this.x = x;
        this.y = y;
    }

    fn getX() {
        return this.x;
    }

    fn getY() {
        return this.y;
    }
}

class Point3D extends Point {
    fn constructor(x, y, z) {
        super(x, y);
        this.z = z;
    }

    fn getZ() {
        return this.z;
    }
}

let p = new Point3D(10, 20, 30);
